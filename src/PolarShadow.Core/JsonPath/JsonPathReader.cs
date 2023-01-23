using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PolarShadow.Core
{
    public ref struct JsonPathReader
    {
        private readonly ReadOnlySpan<byte> _buffer;
        private JsonPathTokenType _tokenType;
        private JsonPathTokenType _lastTokenType;
        private int _currentPosition;
        private bool _inExpressionFilter;
        private bool _isFirstToken;
        private int _segmentStart;
        private int _segmentLength;
        private bool _readFinal;
        private bool _inPropertyNameFilter;
        private bool _inSliceFilter;
        private bool _inIndexFilter;

        public JsonPathReader(ReadOnlySpan<byte> jsonPath)
        {
            if (jsonPath[0] != '$')
            {
                throw new ArgumentException("json path must start with '$'", nameof(jsonPath));
            }
            _buffer = jsonPath;
            _tokenType = _lastTokenType = JsonPathTokenType.None;
            _currentPosition = -1;
            _inExpressionFilter = false;
            _isFirstToken = true;
            _segmentStart = -1;
            _segmentLength = 0;
            _readFinal = false;
            _inPropertyNameFilter = false;
            _inSliceFilter = false;
            _inIndexFilter = false;
        }

        public readonly JsonPathTokenType TokenType => _tokenType;

        public bool Read()
        {
            if (_readFinal)
            {
                throw new InvalidOperationException("read completed");
            }

            _currentPosition++;

            SkipInvaild();

            if (_currentPosition >= _buffer.Length)
            {
                return false;
            }

            if (_isFirstToken)
            {
                ReadFirstRoot();
                _isFirstToken = false;
                return true;
            }

            _lastTokenType = _tokenType;

            switch (_buffer[_currentPosition])
            {
                case JsonPathConstants.Root:
                    _tokenType = JsonPathTokenType.Root;
                    return true;
                case JsonPathConstants.Dot:
                    if (_buffer[_currentPosition + 1] == JsonPathConstants.Dot)
                    {
                        _tokenType = JsonPathTokenType.DeepScan;
                        _currentPosition++;
                    }
                    else
                    {
                        _tokenType = JsonPathTokenType.Child;
                    }
                    return true;
                case JsonPathConstants.StartFilter:
                    if (_lastTokenType == JsonPathTokenType.Root || _lastTokenType == JsonPathTokenType.PropertyName)
                    {
                        _currentPosition++;
                        SkipInvaild();
                        if (_buffer[_currentPosition] == JsonPathConstants.ExpressionStart
                            && _buffer[_currentPosition+1] == JsonPathConstants.LeftBracket)
                        {
                            _currentPosition++;
                            _tokenType = JsonPathTokenType.ExpressionFilterStart;
                            _inExpressionFilter = true;
                        }
                        else if (_buffer[_currentPosition] == JsonPathConstants.SingleQuote)
                        {
                            _tokenType = JsonPathTokenType.PropertyNameFilterStart;
                            _inPropertyNameFilter = true;
                        }
                        else if (_buffer[_currentPosition] == JsonPathConstants.Minus)
                        {
                            if (TryFindNotNumberFirst(out byte ch))
                            {
                                if (ch == JsonPathConstants.Colon)
                                {
                                    _tokenType = JsonPathTokenType.ArraySliceFilterStart;
                                    _inSliceFilter = true;
                                    _currentPosition--;
                                }
                                else if (ch == JsonPathConstants.EndFilter || ch == JsonPathConstants.EndFilter)
                                {
                                    _tokenType = JsonPathTokenType.ArrayIndexFilterStart;
                                    _inIndexFilter = true;
                                    _currentPosition--;
                                }
                                else
                                {
                                    ThrowInvalidDataException();
                                }
                            }
                        }
                        else if (_buffer[_currentPosition] >= JsonPathConstants.Num0 && _buffer[_currentPosition] <= JsonPathConstants.Num9)
                        {
                            if (TryFindNotNumberFirst(out byte ch))
                            {
                                if (ch == JsonPathConstants.Colon)
                                {
                                    _tokenType = JsonPathTokenType.ArraySliceFilterStart;
                                    _inSliceFilter = true;
                                    _currentPosition--;
                                }
                                else if (ch == JsonPathConstants.Comma || ch == JsonPathConstants.EndFilter)
                                {
                                    _tokenType = JsonPathTokenType.ArrayIndexFilterStart;
                                    _inIndexFilter = true;
                                    _currentPosition--;
                                }
                                else
                                {
                                    ThrowInvalidDataException();
                                }
                            }

                        }
                        else if (_buffer[_currentPosition] == JsonPathConstants.Colon)
                        {
                            if (_buffer[_currentPosition + 1] < JsonPathConstants.Num0 || _buffer[_currentPosition + 1] > JsonPathConstants.Num9)
                            {
                                ThrowInvalidDataException();
                                return false;
                            }
                            _tokenType = JsonPathTokenType.ArraySliceFilterStart;
                            _inSliceFilter = true;
                            _currentPosition--;
                        }
                        else if (_buffer[_currentPosition] == JsonPathConstants.Wildcard)
                        {
                            _tokenType = JsonPathTokenType.ArraySliceFilterStart;
                            _inSliceFilter = true;
                            _currentPosition--;
                        }
                        return true;
                    }
                    ThrowInvalidDataException();
                    return false;
                case JsonPathConstants.EndFilter:
                    EndFilter();
                    return true;
                case JsonPathConstants.RightBracket:
                    if (!_inExpressionFilter)
                    {
                        ThrowInvalidDataException();
                    }
                    _currentPosition++;
                    SkipInvaild();
                    if (_buffer[_currentPosition] == JsonPathConstants.EndFilter)
                    {
                        EndFilter();
                        return true;
                    }
                    ThrowInvalidDataException();
                    return false;
                case JsonPathConstants.SelectCurrent:
                    if (!_inExpressionFilter)
                    {
                        ThrowInvalidDataException();
                        return false;
                    }
                    _tokenType = JsonPathTokenType.SelectCurrentNode;
                    return true;
                case JsonPathConstants.Comma:
                    if (_inIndexFilter && _lastTokenType == JsonPathTokenType.Number)
                    {
                        ReadToNextArrayIndexInArrayIndexFilter();
                        ReadNumberSegment();
                        CheckSegment();
                        return true;
                    }
                    else if (_inPropertyNameFilter && _lastTokenType == JsonPathTokenType.PropertyName)
                    {
                        ReadToNextPropertyNameInPropertyNameFilter();
                        ReadPropertySegment();
                        CheckSegment();
                        return true;
                    }
                    ThrowInvalidDataException();
                    return false;
                case JsonPathConstants.Wildcard:
                    if (_lastTokenType == JsonPathTokenType.ArraySliceFilterStart
                        || _lastTokenType == JsonPathTokenType.Child
                        || _lastTokenType == JsonPathTokenType.DeepScan)
                    {
                        _tokenType = JsonPathTokenType.Wildcard;
                        return true;
                    }
                    ThrowInvalidDataException();
                    return false;
                default:
                    if (_inSliceFilter)
                    {
                        ReadSliceSegment();
                        CheckSegment();
                        _tokenType = JsonPathTokenType.SliceRange;
                        return true;
                    }
                    else if (IsNumber())
                    {
                        ReadNumberSegment();
                        CheckSegment();
                        _tokenType = JsonPathTokenType.Number;
                        return true;
                    }
                    else if (_inExpressionFilter && JsonPathConstants.OperatorStartChars.IndexOf(_buffer[_currentPosition]) >= 0)
                    {
                        ReadOperatorSegment();
                        CheckSegment();
                        _tokenType = JsonPathTokenType.Operator;
                        return true;
                    }
                    else if (_lastTokenType == JsonPathTokenType.DeepScan
                            || _tokenType == JsonPathTokenType.Child
                            || _lastTokenType == JsonPathTokenType.PropertyNameFilterStart
                            || _lastTokenType == JsonPathTokenType.PropertyName)
                    {
                        ReadPropertySegment();
                        CheckSegment();
                        _tokenType = JsonPathTokenType.PropertyName;
                        return true;
                    }
                    ThrowInvalidDataException();
                    return false;
            }
        }

        public bool TryReadOperator(out JsonPathExpressionOperator expressionOperator)
        {
            expressionOperator = JsonPathExpressionOperator.None;
            if (_tokenType != JsonPathTokenType.Operator)
            {
                return false;
            }
            if (_inExpressionFilter)
            {
                var operatorSpan = _buffer.Slice(_segmentStart, _segmentLength);
                switch (operatorSpan[0])
                {
                    case JsonPathConstants.Equal:
                        if (operatorSpan.Length == 2)
                        {
                            if (operatorSpan[1] == JsonPathConstants.Equal)
                            {
                                expressionOperator = JsonPathExpressionOperator.Equal;
                                return true;
                            }
                            else if (operatorSpan[1] == JsonPathConstants.MatchRegex)
                            {
                                expressionOperator = JsonPathExpressionOperator.Regex;
                                return true;
                            }
                        }
                        break;
                    case JsonPathConstants.LessThan:
                        if (operatorSpan.Length == 1)
                        {
                            expressionOperator = JsonPathExpressionOperator.LessThan;
                            return true;
                        }
                        else if(operatorSpan.Length == 2 && operatorSpan[1] == JsonPathConstants.Equal)
                        {
                            expressionOperator = JsonPathExpressionOperator.LessThanOrEqual;
                            return true;
                        }
                        break;
                    case JsonPathConstants.GreaterThan:
                        if (operatorSpan.Length == 1)
                        {
                            expressionOperator = JsonPathExpressionOperator.GreaterThan;
                            return true;
                        }
                        else if (operatorSpan.Length == 2 && operatorSpan[1] == JsonPathConstants.Equal)
                        {
                            expressionOperator = JsonPathExpressionOperator.GreaterThanOrEqual;
                            return true;
                        }
                        break;
                    case JsonPathConstants.Not:
                        if (operatorSpan.Length == 2 && operatorSpan[1] == JsonPathConstants.Equal)
                        {
                            expressionOperator = JsonPathExpressionOperator.NotEqual;
                            return true;
                        }
                        break;
                    case (byte)'i':
                        if (operatorSpan.SequenceEqual(JsonPathConstants.InChars))
                        {
                            expressionOperator = JsonPathExpressionOperator.In;
                            return true;
                        }
                        break;
                    case (byte)'n':
                        if (operatorSpan.SequenceEqual(JsonPathConstants.NInChars))
                        {
                            expressionOperator = JsonPathExpressionOperator.NIn;
                            return true;
                        }
                        else if (operatorSpan.SequenceEqual(JsonPathConstants.NoneOfChars))
                        {
                            expressionOperator = JsonPathExpressionOperator.NoneOf;
                            return true;
                        }
                        break;
                    case (byte)'s':
                        if (operatorSpan.SequenceEqual(JsonPathConstants.SubsetOfChars))
                        {
                            expressionOperator = JsonPathExpressionOperator.SubsetOf;
                            return true;
                        }
                        else if (operatorSpan.SequenceEqual(JsonPathConstants.SizeChars))
                        {
                            expressionOperator = JsonPathExpressionOperator.Size;
                            return true;
                        }
                        break;
                    case (byte)'a':
                        if (operatorSpan.SequenceEqual(JsonPathConstants.AnyOfChars))
                        {
                            expressionOperator = JsonPathExpressionOperator.AnyOf;
                            return true;
                        }
                        break;
                    case (byte)'e':
                        if (operatorSpan.SequenceEqual(JsonPathConstants.EmptyChars))
                        {
                            expressionOperator = JsonPathExpressionOperator.Empty;
                            return true;
                        }
                        break;
                }
            }

            expressionOperator = JsonPathExpressionOperator.None;
            return false;
        }
        
        public bool TryReadProperty(out string property)
        {
            if (_tokenType == JsonPathTokenType.PropertyName)
            {
                property = Encoding.UTF8.GetString(_buffer.Slice(_segmentStart, _segmentLength));
                return true;
            }

            property = default;
            return false;
        }

        public bool TryReadNumber(out int num)
        {
            num = 0;

            if (_tokenType == JsonPathTokenType.Number)
            {
                return int.TryParse(Encoding.UTF8.GetString(_buffer.Slice(_segmentStart, _segmentLength)), out num);
            }

            return false;
        }

        public bool TryReadSliceRange(out JsonPathSliceRange range)
        {
            var slice = _buffer.Slice(_segmentStart, _segmentLength);

            var colonIndex = slice.IndexOf(JsonPathConstants.Colon);
            if (colonIndex < 0)
            {
                range = new JsonPathSliceRange(default, default);
                return false;
            }

            var startIndex = SkipInvaild(slice, 0);
            if (startIndex == colonIndex)//":9"
            {
                var endIndex = SkipInvaild(slice, colonIndex + 1);
                if (startIndex == endIndex)//":"
                {
                    range = new JsonPathSliceRange(default, default);
                    return false;
                }
                else 
                {
                    var endLength = ReadUntilNotNumber(slice, endIndex);
                    if (!int.TryParse(Encoding.UTF8.GetString(slice.Slice(endIndex, endLength)), out int end))
                    {
                        range = new JsonPathSliceRange(default, default);
                        return false;
                    }
                    range = new JsonPathSliceRange(default, end);
                    return true;
                }
            }
            else
            {
                var startLength = ReadUntilNotNumber(slice, startIndex);
                if (!int.TryParse(Encoding.UTF8.GetString(slice.Slice(startIndex, colonIndex - startIndex)), out int start))
                {
                    range = new JsonPathSliceRange(default, default);
                    return false;
                }

                var endIndex = SkipInvaild(slice, colonIndex + 1);
                var endLength = ReadUntilNotNumber(slice, endIndex);
                if (endLength == 0)
                {
                    range = new JsonPathSliceRange(start, default);
                    return true;
                }

                if (!int.TryParse(Encoding.UTF8.GetString(slice.Slice(endIndex, endLength)), out int end))
                {
                    range = new JsonPathSliceRange(default, default);
                    return false;
                }
                range = new JsonPathSliceRange(start, end);
                return true;
            }
        }

        private void ReadPropertySegment()
        {
            _segmentStart = _currentPosition;
            _segmentLength = ReadUntiNotProperty();
        }

        private void ReadNumberSegment()
        {
            _segmentStart = _currentPosition;
            _segmentLength = ReadUntilNotNumber();
        }

        private void ReadOperatorSegment()
        {
            _segmentStart = _currentPosition;
            _segmentLength = ReadUntilNotOperator();
        }

        private void ReadSliceSegment()
        {
            _segmentStart = _currentPosition;
            _segmentLength = ReadSliceRange();
        }

        private int ReadUntiNotProperty()
        {
            var start = _currentPosition;
            var next = start + 1;
            while (!IsEnd(next) && JsonPathConstants.PropertyEndChars.IndexOf(_buffer[next]) < 0
                && JsonPathConstants.OperatorStartChars.IndexOf(_buffer[next]) < 0)
            {
                _currentPosition++;
                next++;
            }

            if (!IsEnd(next) && _buffer[next] == JsonPathConstants.SingleQuote)
            {
                _currentPosition = next;
                return _currentPosition - start;
            }

            return _currentPosition - start + 1;
        }

        private int ReadUntilNotNumber()
        {
            var start = _currentPosition;
            var ch = _buffer[++_currentPosition];
            while (_currentPosition < _buffer.Length && (ch >= JsonPathConstants.Num0 && ch <= JsonPathConstants.Num9))
            {
                ch = _buffer[++_currentPosition];
            }
            _currentPosition--;
            return _currentPosition - start + 1;
        }

        private int ReadUntilNotNumber(ReadOnlySpan<byte> buffer, int startIndex)
        {
            var index = startIndex;
            while (index < buffer.Length && (buffer[index] >= JsonPathConstants.Num0 && buffer[index] <= JsonPathConstants.Num9))
            {
                index++;
            }
            return index - startIndex;
        }

        private int ReadUntilNotOperator()
        {
            var start = _currentPosition;
            var ch = _buffer[++_currentPosition];
            while (JsonPathConstants.OperatorChars.IndexOf(ch) >= 0)
            {
                ch = _buffer[++_currentPosition];
            }

            _currentPosition--;
            return _currentPosition - start + 1;
        }

        private int ReadSliceRange()
        {
            var start = _currentPosition;
            var ch = _buffer[++_currentPosition];
            while (ch != JsonPathConstants.EndFilter)
            {
                ch = _buffer[++_currentPosition];
            }

            _currentPosition--;
            return _currentPosition - start + 1; 
        }

        private void ReadToNextPropertyNameInPropertyNameFilter()
        {
            if (_buffer[_currentPosition] == JsonPathConstants.Comma)
            {
                _currentPosition++;
                SkipInvaild();
                if (_buffer[_currentPosition] == JsonPathConstants.SingleQuote)
                {
                    _currentPosition++;
                    SkipInvaild();
                    return;
                }
            }
            else if (_buffer[_currentPosition] == JsonPathConstants.EndFilter)
            {
                _currentPosition--;
                return;
            }
            ThrowInvalidDataException();
        }

        private void ReadToNextArrayIndexInArrayIndexFilter()
        {
            if (_buffer[_currentPosition] == JsonPathConstants.Comma)
            {
                _currentPosition++;
                SkipInvaild();
                if (IsNumber())
                {
                    return;
                }
            }

            ThrowInvalidDataException();
        }

        private bool IsVaildSegment()
        {
            return _segmentLength > 0;
        }
        private void CheckSegment()
        {
            if (!IsVaildSegment())
            {
                ThrowInvalidDataException();
            }
        }

        private void SkipInvaild()
        {
            while (!IsEnd()
                && _buffer[_currentPosition] <= JsonPathConstants.Space 
                && JsonPathConstants.SkipChars.IndexOf(_buffer[_currentPosition]) >= 0)
            {
                _currentPosition++;
            }
        }

        private int SkipInvaild(ReadOnlySpan<byte> buffer, int startindex)
        {
            while (startindex < buffer.Length 
                && buffer[startindex] <= JsonPathConstants.Space
                && JsonPathConstants.SkipChars.IndexOf(_buffer[startindex]) >= 0)
            {
                startindex++;
            }

            return startindex;
        }

        private void ReadFirstRoot()
        {
            if (TryReadRoot())
            {
                _tokenType = JsonPathTokenType.Root;
                return;
            }

            ThrowInvalidDataException();
        }

        private bool IsEnd()
        {
            return _currentPosition >= _buffer.Length;
        }

        private bool IsEnd(int index)
        {
            return index >= _buffer.Length;
        }

        private bool TryReadRoot()
        {
            return _buffer[_currentPosition] == JsonPathConstants.Root;
        }

        private void ThrowInvalidDataException()
        {
            _readFinal = true;
            throw new InvalidDataException();
        }

        private bool IsRemainEnough(int requireRemain)
        {
            return _buffer.Length - 1 - _currentPosition >= requireRemain;
        }

        private void EndFilter()
        {
            if (_inPropertyNameFilter)
            {
                _tokenType = JsonPathTokenType.PropertyNameFilterEnd;
                _inPropertyNameFilter = false;
                return;
            }
            else if (_inSliceFilter)
            {
                _tokenType = JsonPathTokenType.ArraySliceFilterEnd;
                _inSliceFilter = false;
                return;
            }
            else if (_inIndexFilter)
            {
                _tokenType = JsonPathTokenType.ArrayIndexFilterEnd;
                _inIndexFilter = false;
                return;
            }
            else if (_inExpressionFilter)
            {
                _tokenType = JsonPathTokenType.ExpressionFilterEnd;
                _inExpressionFilter = false;
                return;
            }

            ThrowInvalidDataException();
        }

        private bool TryFindNotNumberFirst(out byte ch)
        {
            ch = 0;
            var pos = _currentPosition;
            while (++pos < _buffer.Length)
            {
                if (_buffer[pos] >= JsonPathConstants.Num0 && _buffer[pos] <= JsonPathConstants.Num9)
                {
                    continue;
                }
                ch = _buffer[pos];
                return true;
            }
            return false;
        }

        private bool IsNumber()
        {
            return _buffer[_currentPosition] == JsonPathConstants.Minus || (_buffer[_currentPosition] >= JsonPathConstants.Num0 && _buffer[_currentPosition] <= JsonPathConstants.Num9);
        }
    }
}
