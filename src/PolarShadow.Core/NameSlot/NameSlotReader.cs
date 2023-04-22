using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal ref partial struct NameSlotReader
    {
        private readonly ReadOnlySpan<byte> _buffer;

        private int _index;

        private NameSlotTokenType _tokenType;
        private int _segmentStart;
        private int _segmentEnd;

        private bool _readClose;

        public NameSlotReader(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            _index = 0;
            _tokenType = NameSlotTokenType.None;
            _segmentStart = _segmentEnd = -1;
            _readClose = false;
        }

        public NameSlotTokenType TokenType => _tokenType;


        public bool Read()
        {
            Skip();
            if (!CanRead()) return ReadClose();

            if (_tokenType == NameSlotTokenType.None || _tokenType == NameSlotTokenType.End)
            {
                _readClose = !ReadToStart();
                return !_readClose;
            }

            if (!ReadNext())
            {
                return ReadNone();
            }
            return true;
        }

        private bool ReadToStart()
        {
            if (ReadtoChar(NameSlotConstants.SlotStart))
            {
                _segmentStart = _segmentEnd = _index;
                _tokenType = NameSlotTokenType.Start;
                return true;
            }
            return false;
        }

        private bool ReadNext()
        {
            var ch = _buffer[_index];
            switch (_tokenType)
            {
                case NameSlotTokenType.None:
                    return ReadToNoneNext();
                case NameSlotTokenType.Start:
                    return ReadToStartNext();
                case NameSlotTokenType.End:
                    break;
                case NameSlotTokenType.Format:
                    break;
                case NameSlotTokenType.Parameter:
                    break;
                case NameSlotTokenType.Property:
                    break;
                case NameSlotTokenType.jsonPath:
                    break;
                case NameSlotTokenType.xPath:
                    break;
                case NameSlotTokenType.Number:
                    break;
                case NameSlotTokenType.Regex:
                    break;
                case NameSlotTokenType.ConditionalExpression:
                    break;
                default:
                    break;
            }
            if (_tokenType == NameSlotTokenType.Property || _tokenType == NameSlotTokenType.jsonPath || _tokenType == NameSlotTokenType.xPath)
            {
            }

            return false;
        }

        private bool ReadToNoneNext()
        {
            var ch = _buffer[_index];
            if (ch == NameSlotConstants.SlotEnd)
            {
                return ReadEnd();
            }
            else if (ch == NameSlotConstants.Divide)
            {
                return true;
            }
            return false;
        }

        private bool ReadToPropertyNext()
        {
            var ch = _buffer[_index];
            if (ch == NameSlotConstants.Colon)
            {
                return ReadFormatStart();
            }
            else if (ch == NameSlotConstants.SlotEnd)
            {
                return ReadEnd();
            }
            return false;
        }

        private bool ReadToStartNext()
        {
            var ch = _buffer[_index];
            if (IsParameterStart(ch))
            {
                return ReadToParameterEnd();
            }
            else if (ch == NameSlotConstants.JsonPathRoot)
            {
                return ReadToJsonPathEnd();
            }
            else if (ch == NameSlotConstants.Divide)
            {
                return ReadToXPathEnd();
            }
            return false;
        }

        private bool ReadRegexEnd()
        {
            Skip();
            while (CanRead())
            {
                return false;
            }
            _tokenType = NameSlotTokenType.Regex;
            _segmentStart = _segmentEnd = _index;
            return false;
        }

        private bool ReadFormatStart()
        {
            _tokenType = NameSlotTokenType.Format;
            _segmentStart = _segmentEnd = _index;
            return true;
        }

        private bool ReadToFormatEnd()
        {
            Skip();
            _segmentStart = _segmentEnd = _index;
            while (CanRead())
            {
                if (IsFormatEnd(_buffer[_index]))
                {
                    _tokenType = NameSlotTokenType.Format;
                    _segmentEnd = _index - 1;
                    return true;
                }
                _index++;
            }

            return false;
        }

        private bool ReadEnd()
        {
            _tokenType = NameSlotTokenType.End;
            _segmentStart = _segmentEnd = _index;
            return true;
        }

        private bool ReadtoChar(byte ch)
        {
            while (CanRead())
            {
                if (_buffer[_index] == ch)
                {
                    return true;
                }
                _index++;
            }

            return false;
        }

        private bool NextCharIs(byte ch)
        {
            var nextIndex = _index + 1;
            if (nextIndex >= _buffer.Length) return false;
            return _buffer[nextIndex] == ch;
        }

        private void Skip()
        {
            while (CanRead() && _buffer[_index] <= NameSlotConstants.Space)
            {
                _index++;
            }
        }

        private bool CanRead()
        {
            return _index < _buffer.Length && !_readClose;
        }

        private bool ReadNone()
        {
            _tokenType = NameSlotTokenType.None;
            return true;
        }

        private bool ReadClose()
        {
            _readClose = true;
            _tokenType = NameSlotTokenType.None;
            _segmentEnd = _segmentStart = -1;
            return false;
        }

        private bool IsFormatEnd(byte ch)
        {
            return ch == NameSlotConstants.Space || ch == NameSlotConstants.SlotEnd;
        }
    }
}
