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

        private bool _readEnd;

        public NameSlotReader(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            _index = 0;
            _tokenType = NameSlotTokenType.None;
            _segmentStart = _segmentEnd = -1;
            _readEnd = false;
        }

        public NameSlotTokenType TokenType => _tokenType;


        public bool Read()
        {
            if (CanRead())
            {
                return false;
            }

            if(_tokenType == NameSlotTokenType.None || _tokenType == NameSlotTokenType.End)
            {
                _readEnd = !ReadToStart();
                return !_readEnd;
            }

            Skip();
            if (!CanRead())
            {
                return ReadClose();
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
            if (_tokenType == NameSlotTokenType.Start)
            {
                if (IsParameterStart(ch))
                {
                    return ReadToPropertyEnd();
                }
                else if (ch == NameSlotConstants.JsonPathRoot)
                {
                    return ReadToJsonPathEnd();
                }
                else if (ch == NameSlotConstants.Tilde)
                {

                }
            }
            else if (_tokenType == NameSlotTokenType.Property || _tokenType == NameSlotTokenType.jsonPath || _tokenType == NameSlotTokenType.xPath)
            {
                if (ch == NameSlotConstants.Colon)
                {
                    return ReadFormatStart();
                }
                else if (ch == NameSlotConstants.SlotEnd)
                {
                    return ReadEnd();
                }
            }
            else if (_tokenType == NameSlotTokenType.None)
            {
                if (ch == NameSlotConstants.SlotEnd)
                {
                    return ReadEnd();
                }
                else if (ch == NameSlotConstants.Divide)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ReadRegexEnd()
        {
            Skip();
            while (CanRead())
            {
                return true;
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

        private bool ReadToPropertyEnd()
        {
            _segmentStart = _segmentEnd = _index;
            while (CanRead())
            {
                if (IsParameterChar(_buffer[_index]))
                {
                    _index++;
                }
                else
                {
                    _tokenType = NameSlotTokenType.Property;
                    _segmentEnd = _index - 1;
                    return true;
                }
            }

            return false;
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
            return _index < _buffer.Length && !_readEnd;
        }

        private bool ReadNone()
        {
            _tokenType = NameSlotTokenType.None;
            return true;
        }

        private bool ReadClose()
        {
            _readEnd = true;
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
