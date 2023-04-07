using HtmlAgilityPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;

namespace PolarShadow.Core
{
    public ref struct JsonPathReader
    {
        private readonly ReadOnlySpan<byte> _buffer;
        private JsonPathTokenType _tokenType;
        private int _currentPosition;
        private bool _inExpression;
        private int _segmentStart;
        private int _segmentEnd;
        private bool _readFinal;

        public JsonPathReader(ReadOnlySpan<byte> jsonPath)
        {
            if (jsonPath[0] != '$')
            {
                throw new ArgumentException("json path must start with '$'", nameof(jsonPath));
            }
            _buffer = jsonPath;
            _tokenType = JsonPathTokenType.None;
            _currentPosition = -1;
            _inExpression = false;
            _segmentStart = _segmentEnd = -1;
            _readFinal = false;
        }

        public JsonPathReader(ReadOnlySpan<byte> buffer, int start) : this(buffer.Slice(start))
        {

        }

        public JsonPathReader(ReadOnlySpan<byte> buffer, int start, int length) : this(buffer.Slice(start, length))
        {

        }

        public readonly JsonPathTokenType TokenType => _tokenType;


        public bool Read()
        {
            if (_readFinal)
            {
                throw new InvalidOperationException("read completed");
            }

            _currentPosition++;
            if (ShouldSkipInvaild())
            SkipInvaild();

            if (IsEnd()) return false;

            if (_tokenType == JsonPathTokenType.None)
            {
                if (!ReadRoot())
                {
                    ThrowInvalidDataException();
                }

                return true;
            }
            if (!ReadNext())
            {
                _readFinal = true;
                ThrowInvalidDataException();
                return false;
            }

            return true;
        }

        public string GetString()
        {
            return Encoding.UTF8.GetString(_buffer.Slice(_segmentStart, _segmentEnd - _segmentStart + 1));
        }

        public bool TryGetString(out string value)
        {
            value = string.Empty;
            try
            {
                value = Encoding.UTF8.GetString(_buffer.Slice(_segmentStart, _segmentEnd - _segmentStart + 1));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetInt()
        {
            return int.Parse(GetString());
        }

        public bool TryGetInt(out int value)
        {
            value = 0;
            return TryGetString(out string s) && int.TryParse(s, out value);
        }

        public double GetDouble()
        {
            return double.Parse(GetString());
        }

        public bool TryGetDouble(out double value)
        {
            value = 0d;
            return TryGetString(out string s) && double.TryParse(s, out value);
        }

        public decimal GetDecimal()
        {
            return decimal.Parse(GetString());
        }

        public bool TryGetDecimal(out decimal value)
        {
            value = 0;
            return TryGetString(out string s) && decimal.TryParse(s, out value);
        }

        private bool ReadNext()
        {
            switch (_tokenType)
            {
                case JsonPathTokenType.Root:
                    return ReadRootNext();
                case JsonPathTokenType.Current:
                    return ReadSelectCurrentNext();
                case JsonPathTokenType.Wildcard:
                    return ReadWildcardNext();
                case JsonPathTokenType.DeepScan:
                case JsonPathTokenType.Child:
                    return ReadChildOrDeepScanNext();
                case JsonPathTokenType.PropertyName:
                    return ReadPropertyNameNext();
                case JsonPathTokenType.String:
                    return ReadStringNext();
                case JsonPathTokenType.StartFilter:
                    return ReadStartFilterNext();
                case JsonPathTokenType.EndFilter:
                    return ReadEndFilterNext();
                case JsonPathTokenType.Number:
                    return ReadNumberNext();
                case JsonPathTokenType.Slice:
                    return ReadSliceNext();
                case JsonPathTokenType.StartExpression:
                    return ReadStartExpressionNext();
                case JsonPathTokenType.EndExpression:
                    return ReadEndExpressionNext();
                default:
                    return ReadOperateNext();
            }
        }

        private bool ReadRootNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.StartFilter)
            {
                _tokenType = JsonPathTokenType.StartFilter;
                return true;
            }
            return TryReadChildOrDeepScan();
        }

        private bool ReadStartFilterNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.SingleQuote)
            {
                _tokenType = JsonPathTokenType.String;
                return ReadStringEnd();
            }
            else if (IsNumberCharStart(ch))
            {
                _tokenType = JsonPathTokenType.Number;
                return ReadIntegerEnd();
            }
            else if (ch == JsonPathConstants.Colon)
            {
                _tokenType = JsonPathTokenType.Slice;
                return true;
            }
            else if (ch == JsonPathConstants.Wildcard)
            {
                _tokenType = JsonPathTokenType.Wildcard;
                return true;
            }

            return TryReadStartExpression();
        }

        private bool ReadEndFilterNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.Dot && NextCharIs(JsonPathConstants.Dot))
            {
                _tokenType = JsonPathTokenType.DeepScan;
                return true;
            }
            else if (ch == JsonPathConstants.Dot)
            {
                _tokenType = JsonPathTokenType.Child;
                return true;
            }
            else if (ch == JsonPathConstants.RightBracket)
            {
                _tokenType = JsonPathTokenType.EndExpression;
                return true;
            }
            return false;
        }

        private bool ReadPropertyNameNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.RightBracket)
            {
                return ReadEndExpression();
            }
            else if (!_inExpression && ch == JsonPathConstants.StartFilter)
            {
                _tokenType = JsonPathTokenType.StartFilter;
                return true;
            }
            else if (_inExpression)
            {
                SkipInvaild();
                return ReadInExpressionPropertyNameEndNext();
            }

            return TryReadChildOrDeepScan();
        }

        private bool ReadInExpressionPropertyNameEndNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.Equal && NextCharIs(JsonPathConstants.Equal))
            {
                _tokenType = JsonPathTokenType.Equal;
                return true;
            }
            else if (ch == JsonPathConstants.LessThan && NextCharIs(JsonPathConstants.Equal))
            {
                _tokenType = JsonPathTokenType.LessThenOrEqual;
                _currentPosition++;
                return true;
            }
            else if (ch == JsonPathConstants.GreaterThan && NextCharIs(JsonPathConstants.Equal))
            {
                _tokenType = JsonPathTokenType.GreaterThanOrEqual;
                _currentPosition++;
                return true;
            }
            else if (ch == JsonPathConstants.Not && NextCharIs(JsonPathConstants.Equal))
            {
                _tokenType = JsonPathTokenType.NotEqual;
                _currentPosition++;
                return true;
            }
            else if (ch == JsonPathConstants.Equal && NextCharIs(JsonPathConstants.MatchRegex))
            {
                _tokenType = JsonPathTokenType.Matches;
                _currentPosition++;
                return true;
            }
            else if (ch == JsonPathConstants.LessThan)
            {
                _tokenType = JsonPathTokenType.LessThan;
                return true;
            }
            else if (ch == JsonPathConstants.GreaterThan)
            {
                _tokenType = JsonPathTokenType.GreaterThan;
                return true;
            }
            else if (StartsWithAndEndWithSpace(JsonPathConstants.InChars))
            {
                _tokenType = JsonPathTokenType.In;
                _currentPosition += JsonPathConstants.InChars.Length;
                return true;
            }
            else if (StartsWithAndEndWithSpace(JsonPathConstants.NInChars))
            {
                _tokenType = JsonPathTokenType.Nin;
                _currentPosition += JsonPathConstants.NInChars.Length;
                return true;
            }
            else if (StartsWithAndEndWithSpace(JsonPathConstants.SubsetOfChars))
            {
                _tokenType = JsonPathTokenType.Subsetof;
                _currentPosition += JsonPathConstants.SubsetOfChars.Length;
                return true;
            }
            else if (StartsWithAndEndWithSpace(JsonPathConstants.AnyOfChars))
            {
                _tokenType = JsonPathTokenType.Anyof;
                _currentPosition += JsonPathConstants.AnyOfChars.Length;
                return true;
            }
            else if (StartsWithAndEndWithSpace(JsonPathConstants.NoneOfChars))
            {
                _tokenType = JsonPathTokenType.Noneof;
                _currentPosition += JsonPathConstants.NoneOfChars.Length;
                return true;
            }
            else if (StartsWithAndEndWithSpace(JsonPathConstants.SizeChars))
            {
                _tokenType = JsonPathTokenType.Size;
                _currentPosition += JsonPathConstants.SizeChars.Length;
                return true;
            }
            else if (StartsWithAndEndWithSpace(JsonPathConstants.EmptyChars))
            {
                _tokenType = JsonPathTokenType.Empty;
                _currentPosition += JsonPathConstants.EmptyChars.Length;
                return true;
            }

            return false;
        }

        private bool ReadOperateNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.Root)
            {
                _tokenType = JsonPathTokenType.Root;
                return true;
            }
            else if (ch == JsonPathConstants.SelectCurrent)
            {
                _tokenType = JsonPathTokenType.Current;
                return true;
            }
            else if (IsNumberCharStart(ch))
            {
                _tokenType = JsonPathTokenType.Number;
                return ReadNumberEnd();
            }
            else if (ch == JsonPathConstants.RegexStart)
            {
                _tokenType = JsonPathTokenType.Regex;
                return ReadRegexEnd();
            }

            return false;
        }

        private bool ReadStringNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.Comma)
            {
                _currentPosition++;
                SkipInvaild();
                if (_buffer[_currentPosition] == JsonPathConstants.SingleQuote)
                {
                    _tokenType = JsonPathTokenType.String;
                    return ReadStringEnd();
                }
            }
            else if (ch == JsonPathConstants.EndFilter)
            {
                _tokenType = JsonPathTokenType.EndFilter;
                return true;
            }
            else if (ch == JsonPathConstants.RightBracket)
            {
                return ReadEndExpression();
            }

            return false;
        }

        private bool ReadNumberNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.Comma)
            {
                _currentPosition++;
                SkipInvaild();
                if (IsNumberCharStart(_buffer[_currentPosition]))
                {
                    _tokenType = JsonPathTokenType.Number;
                    return ReadIntegerEnd();
                }
            }
            else if (ch == JsonPathConstants.EndFilter)
            {
                _tokenType = JsonPathTokenType.EndFilter;
                return true;
            }
            else if (ch == JsonPathConstants.RightBracket)
            {
                return ReadEndExpression();
            }
            else if (ch == JsonPathConstants.Colon)
            {
                _tokenType = JsonPathTokenType.Slice;
                return true;
            }

            return false;
        }

        private bool ReadChildOrDeepScanNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.Wildcard)
            {
                _tokenType = JsonPathTokenType.Wildcard;
                return true;
            }
            else if (IsPropertyNameChar(ch))
            {
                _tokenType = JsonPathTokenType.PropertyName;
                return ReadPropertyNameEnd();
            }
            else if (ch == JsonPathConstants.StartFilter)
            {
                _tokenType = JsonPathTokenType.StartFilter;
                return true;
            }

            return false;
        }

        private bool ReadWildcardNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.StartFilter)
            {
                _tokenType = JsonPathTokenType.StartFilter;
                return true;
            }
            else if (ch == JsonPathConstants.EndFilter)
            {
                _tokenType = JsonPathTokenType.EndFilter;
                return true;
            }
            return TryReadChildOrDeepScan();
        }

        private bool ReadSliceNext()
        {
            var ch = _buffer[_currentPosition];
            if (IsNumberCharStart(ch))
            {
                _tokenType = JsonPathTokenType.Number;
                return ReadIntegerEnd();
            }
            else if (ch == JsonPathConstants.EndFilter)
            {
                _tokenType = JsonPathTokenType.EndFilter;
                return true;
            }

            return false;
        }

        private bool ReadStartExpressionNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.SelectCurrent)
            {
                _tokenType = JsonPathTokenType.Current;
                return true;
            }
            else if (ch == JsonPathConstants.Root)
            {
                _tokenType = JsonPathTokenType.Root;
                return true;
            }
            return false;
        }

        private bool ReadEndExpressionNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.EndFilter)
            {
                _tokenType = JsonPathTokenType.EndFilter;
                return true;
            }

            return false;
        }

        private bool ReadSelectCurrentNext()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.Dot && NextCharIs(JsonPathConstants.Dot))
            {
                _tokenType = JsonPathTokenType.DeepScan;
                return true;
            }
            else if (ch == JsonPathConstants.Dot)
            {
                _tokenType = JsonPathTokenType.Child;
                return true;
            }

            return false;
        }

        private bool NextCharIs(byte ch)
        {
            return NextCharIs(_currentPosition, ch);
        }

        private bool NextCharIs(int start, byte ch)
        {
            var nextIndex = start + 1;
            if (nextIndex >= _buffer.Length)
            {
                return false;
            }
            return _buffer[nextIndex] == ch;
        }

        private bool NextCharIsIn(ReadOnlySpan<byte> chs)
        {
            return NextCharIsIn(_currentPosition, chs);
        }

        private bool NextCharIsIn(int start, ReadOnlySpan<byte> chs)
        {
            var nextIndex = start + 1;
            if (nextIndex >= _buffer.Length)
            {
                return false;
            }

            return chs.IndexOf(_buffer[nextIndex]) > -1;
        }

        private bool ReadPropertyNameEnd()
        {
            _segmentStart = _segmentEnd = _currentPosition;
            _currentPosition++;
            while (!IsEnd() && IsPropertyNameChar(_buffer[_currentPosition]))
            {
                _currentPosition++;
            }
            _segmentEnd = --_currentPosition;
            return _segmentEnd >= _segmentStart;
        }

        private bool ReadStringEnd()
        {
            _currentPosition++;
            _segmentStart = _segmentEnd = _currentPosition;
            while (!IsEnd() && IsPropertyNameChar(_buffer[_currentPosition]))
            {
                _currentPosition++;
            }
            _segmentEnd = _currentPosition - 1;

            if (_buffer[_currentPosition] == JsonPathConstants.SingleQuote && _segmentStart <= _segmentEnd)
            {
                return true;
            }

            return false;
        }

        private bool ReadIntegerEnd()
        {
            _segmentStart = _segmentEnd = _currentPosition;
            _currentPosition++;
            while (!IsEnd() && IsIntegerChar(_buffer[_currentPosition]))
            {
                _currentPosition++;
            }
            _segmentEnd = --_currentPosition;
            if (_segmentStart == _segmentEnd && _buffer[_segmentStart] != JsonPathConstants.Minus
                || _segmentEnd > _segmentStart)
            {
                return true;
            }

            return false;
        }

        private bool ReadNumberEnd()
        {
            _segmentStart = _segmentEnd = _currentPosition;
            _currentPosition++;
            bool hasdot = false;
            while (!IsEnd() && IsNumberChar(_buffer[_currentPosition]))
            {
                if (_buffer[_currentPosition] == JsonPathConstants.Dot)
                {
                    if (!hasdot)
                    {
                        hasdot = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                _currentPosition++;
            }
            _segmentEnd = --_currentPosition;

            if (_segmentStart == _segmentEnd
                && _buffer[_segmentStart] != JsonPathConstants.Minus
                || _segmentEnd > _segmentStart && _buffer[_segmentEnd] != JsonPathConstants.Dot)
            {
                return true;
            }

            return false;
        }

        private bool ReadRegexEnd()
        {
            _segmentStart = _segmentEnd = _currentPosition;
            _currentPosition++;
            while (!IsEnd())
            {
                if (_buffer[_currentPosition] == JsonPathConstants.RegexStart)
                {
                    if (!NextCharIs(JsonPathConstants.RightBracket)
                        && !NextCharIs(JsonPathConstants.Space)
                        && NextCharIsIn(JsonPathConstants.RegexModifyChars))
                    {
                        _currentPosition++;
                    }

                    _segmentEnd = _currentPosition;
                    return true;
                }
                _currentPosition++;
            }

            return false;
        }

        private bool TryReadStartExpression()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.ExpressionStart && NextCharIs(JsonPathConstants.LeftBracket))
            {
                _currentPosition++;
                _tokenType = JsonPathTokenType.StartExpression;
                _inExpression = true;
                return true;
            }

            return false;
        }

        private bool ReadEndExpression()
        {
            _tokenType = JsonPathTokenType.EndExpression;
            _inExpression = false;
            return true;
        }

        private bool TryReadChildOrDeepScan()
        {
            var ch = _buffer[_currentPosition];
            if (ch == JsonPathConstants.Dot && NextCharIs(JsonPathConstants.Dot))
            {
                _tokenType = JsonPathTokenType.DeepScan;
                _currentPosition++;
                return true;
            }
            else if (ch == JsonPathConstants.Dot)
            {
                _tokenType = JsonPathTokenType.Child;
                return true;
            }
            return false;
        }

        private bool IsPropertyNameChar(byte ch)
        {
            return ch >= JsonPathConstants.A && ch <= JsonPathConstants.Z
                || ch >= JsonPathConstants.a && ch <= JsonPathConstants.z
                || ch == JsonPathConstants.UnderLine;
        }

        private bool IsNumberCharStart(byte ch)
        {
            return ch == JsonPathConstants.Minus
                || ch >= JsonPathConstants.Num0 && ch <= JsonPathConstants.Num9;
        }

        private bool IsIntegerChar(byte ch)
        {
            return ch >= JsonPathConstants.Num0 && ch <= JsonPathConstants.Num9;
        }

        private bool IsNumberChar(byte ch)
        {
            return IsIntegerChar(ch) || ch == JsonPathConstants.Dot;
        }

        private bool StartsWithAndEndWithSpace(ReadOnlySpan<byte> buffer)
        {
            return _buffer.Slice(_currentPosition).StartsWith(buffer) && NextCharIs(_currentPosition + buffer.Length, JsonPathConstants.Space);
        }

        private void SkipInvaild()
        {
            while (!IsEnd()
                && _buffer[_currentPosition] == JsonPathConstants.Space)
            {
                _currentPosition++;
            }
        }

        private bool ShouldSkipInvaild()
        {
            return _tokenType == JsonPathTokenType.Equal
                || _tokenType == JsonPathTokenType.GreaterThan
                || _tokenType == JsonPathTokenType.LessThan
                || _tokenType == JsonPathTokenType.GreaterThanOrEqual
                || _tokenType == JsonPathTokenType.LessThenOrEqual
                || _tokenType == JsonPathTokenType.In
                || _tokenType == JsonPathTokenType.Nin
                || _tokenType == JsonPathTokenType.Noneof
                || _tokenType == JsonPathTokenType.Anyof
                || _tokenType == JsonPathTokenType.Empty
                || _tokenType == JsonPathTokenType.EndExpression
                || _tokenType == JsonPathTokenType.Matches
                || _tokenType == JsonPathTokenType.NotEqual
                || _tokenType == JsonPathTokenType.Number
                || _tokenType == JsonPathTokenType.Size
                || _tokenType == JsonPathTokenType.Slice
                || _tokenType == JsonPathTokenType.StartExpression
                || _tokenType == JsonPathTokenType.Subsetof
                || _tokenType == JsonPathTokenType.StartFilter;
        }

        private bool ReadRoot()
        {
            if (_buffer[_currentPosition] == JsonPathConstants.Root)
            {
                _tokenType = JsonPathTokenType.Root;
                return true;
            }
            return false;
        }

        private bool IsEnd()
        {
            return _currentPosition >= _buffer.Length;
        }

        private void ThrowInvalidDataException()
        {
            _readFinal = true;
            throw new InvalidDataException($"Invalid data at {GetString()}");
        }
    }
}
