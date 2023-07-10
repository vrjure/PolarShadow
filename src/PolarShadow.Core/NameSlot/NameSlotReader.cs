using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Core
{
    public ref partial struct NameSlotReader
    {
        private readonly ReadOnlySpan<byte> _buffer;
        private readonly Stack<NameSlotReaderState> _his;

        private int _index;

        private NameSlotTokenType _tokenType;
        private int _segmentStart;
        private int _segmentEnd;

        public NameSlotReader(string text) : this(Encoding.UTF8.GetBytes(text))
        {

        }

        public NameSlotReader(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            _index = -1;
            _tokenType = NameSlotTokenType.None;
            _segmentStart = _segmentEnd = -1;
            _his = new Stack<NameSlotReaderState>();
        }

        public NameSlotTokenType TokenType => _tokenType;

        public string GetString()
        {
            return Encoding.UTF8.GetString(Slice());
        }

        public ReadOnlySpan<byte> GetSegment()
        {
            if (_tokenType == NameSlotTokenType.None)
            {
                return ReadOnlySpan<byte>.Empty;
            }
            return Slice();
        }

        private ReadOnlySpan<byte> Slice()
        {
            return _buffer.Slice(_segmentStart, _segmentEnd - _segmentStart + 1);
        }

        public bool Read()
        {
            if (_his.TryPop(out NameSlotReaderState state))
            {
                return SetState(state);
            }

            if (!CanRead()) return false;

            var stack = new Stack<NameSlotReaderState>();
            while (Advance())
            {
                if (ReadNext())
                {
                    stack.Push(new NameSlotReaderState(_segmentStart, _segmentEnd, _tokenType));
                }
                else
                {
                    while (stack.TryPop(out state) && state.TokenType != NameSlotTokenType.Text);

                    if (ReadNoneNext() && _tokenType == NameSlotTokenType.Start)
                    {
                        _segmentEnd--;
                    }

                    _segmentStart = state.SegmentStart;
                    _tokenType = NameSlotTokenType.Text;
                    stack.Push(new NameSlotReaderState(_segmentStart, _segmentEnd, _tokenType));
                }
            }

            while(stack.TryPop(out state))
            {
                _his.Push(state);
            }
            
            return SetState(_his.Pop());
        }

        private bool SetState(NameSlotReaderState state)
        {
            _segmentStart = state.SegmentStart;
            _segmentEnd = state.SegmentEnd;
            _tokenType = state.TokenType;
            return true;
        }

        private bool Advance()
        {
            _index++;
            if (ShouldSkip())
                Skip();

            if (!CanRead()) return ReadClose();
            return true;
        }

        private bool ReadNext()
        {
            switch (_tokenType)
            {
                case NameSlotTokenType.None:
                    return ReadNoneNext();
                case NameSlotTokenType.Start:
                    return ReadStartNext();
                case NameSlotTokenType.End:
                    return ReadEndNext();
                case NameSlotTokenType.Format:
                    return ReadFormatNext();
                case NameSlotTokenType.Match:
                    return ReadMatchNext();
                case NameSlotTokenType.Parameter:
                    return ReadParameterNext();
                case NameSlotTokenType.Text:
                    return ReadTextNext();
            }

            return false;
        }

        private bool ReadNoneNext()
        {
            _segmentStart = _segmentEnd = _index;
            if (ReadtoChar(NameSlotConstants.SlotStart) && !NextCharIs(NameSlotConstants.SlotStart))
            {
                if(_segmentStart == _index)
                {
                    _tokenType = NameSlotTokenType.Start;
                    return true;
                }
            }

            _segmentEnd = --_index;
            _tokenType = NameSlotTokenType.Text;
            return true;
        }


        private bool ReadParameterNext()
        {
            var ch = _buffer[_index];
            if (ch == NameSlotConstants.Colon && NextCharIs(NameSlotConstants.Divide))
            {
                _index++;
                return ReadMatchEnd();
            }
            else if (ch == NameSlotConstants.Colon)
            {
                return ReadFormatEnd();
            }
            else if (ch == NameSlotConstants.SlotEnd)
            {
                return ReadEnd();
            }
            return false;
        }

        private bool ReadStartNext()
        {
            var ch = _buffer[_index];

            if (IsParameterStart(ch))
            {
                _tokenType = NameSlotTokenType.Parameter;
                return ReadParameterEnd();
            }
            else if (ch == NameSlotConstants.JsonPathRoot)
            {
                _tokenType = NameSlotTokenType.Parameter;
                return ReadJsonPathEnd();
            }
            else if (ch == NameSlotConstants.Divide)
            {
                _tokenType = NameSlotTokenType.Parameter;
                return ReadToXPathEnd();
            }
            return false;
        }

        private bool ReadEndNext()
        {
            return ReadNoneNext();
        }

        private bool ReadTextNext()
        {
            var ch = _buffer[_index];
            _segmentStart = _segmentEnd = _index;
            if (ch == NameSlotConstants.SlotStart)
            {
                _tokenType = NameSlotTokenType.Start;
                return true;
            }

            return false;
        }

        private bool ReadFormatNext()
        {
            var ch = _buffer[_index];
            if (ch == NameSlotConstants.SlotEnd)
            {
                return ReadEnd();
            }

            return false;
        }

        private bool ReadMatchNext()
        {
            var ch = _buffer[_index];
            if (ch == NameSlotConstants.SlotEnd)
            {
                return ReadEnd();
            }
            return false;
        }

        private bool ReadMatchEnd()
        {
            _tokenType = NameSlotTokenType.Match;
            _segmentStart = _segmentEnd = _index;
            _index++;
            while (CanRead())
            {
                if (_buffer[_index] == NameSlotConstants.Divide && !NextCharIs(NameSlotConstants.Divide))
                {
                    if (NextCharIsIn(NameSlotConstants.RegexModifyChars))
                    {
                        _index++;
                    }

                    _segmentEnd = _index;
                    return true;
                }
                _index++;
            }
            return false;
        }

        private bool ReadFormatEnd()
        {
            _index++;
            Skip();
            _segmentStart = _segmentEnd = _index;
            while (CanRead())
            {
                if (IsFormatEnd(_buffer[_index]))
                {
                    _tokenType = NameSlotTokenType.Format;
                    _segmentEnd = --_index;
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

        private bool NextCharIsIn(ReadOnlySpan<byte> chs)
        {
            var nextIndex = _index + 1;
            if (nextIndex >= _buffer.Length)
            {
                return false;
            }

            return chs.IndexOf(_buffer[nextIndex]) > -1;
        }

        private void Skip()
        {
            while (CanRead() && _buffer[_index] <= NameSlotConstants.Space)
            {
                _index++;
            }
        }

        private bool ShouldSkip()
        {
            return !(_tokenType == NameSlotTokenType.None
                || _tokenType == NameSlotTokenType.End);
        }

        private bool CanRead()
        {
            return _index < _buffer.Length;
        }

        private bool ReadNone()
        {
            _tokenType = NameSlotTokenType.None;
            return true;
        }

        private bool ReadClose()
        {
            _tokenType = NameSlotTokenType.None;
            _segmentEnd = _segmentStart = -1;
            _index = _buffer.Length;
            return false;
        }

        private bool IsFormatEnd(byte ch)
        {
            return ch == NameSlotConstants.Space || ch == NameSlotConstants.SlotEnd;
        }
    }
}
