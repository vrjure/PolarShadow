using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PolarShadow.Core
{
    internal ref struct NameSlot
    {
        private readonly ReadOnlySpan<byte> _buffer;
        private int _currentPosition;
        private NameSlotOperator _operator;
        private NameSlotToken _leftToken;
        private NameSlotToken _rightToken;

        public NameSlot(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            _currentPosition = 0;
            _leftToken = _rightToken = default;
            _operator = NameSlotOperator.None;

            Handle();
        }

        public NameSlotToken LeftToken => _leftToken;
        public NameSlotToken RightToken => _rightToken;
        public NameSlotOperator Operator => _operator;

        private void Handle()
        {
            if (_buffer[_currentPosition] == NameSlotConstants.SlotStart)
            {
                _currentPosition++;
            }
            while (_currentPosition < _buffer.Length)
            {
                SkipInvaild();
                if (IsPropertyStartChar(_buffer[_currentPosition]))
                {
                    if (_operator != NameSlotOperator.None)
                    {
                        throw new InvalidDataException();
                    }
                    var propertyStart = _currentPosition;
                    if (_buffer[_currentPosition] == NameSlotConstants.JsonPathRoot)
                    {
                        _currentPosition++;
                        ReadJsonPathEnd();
                    }
                    else
                    {
                        ReadUntilEnd();
                    }
                    var propertyEnd = _currentPosition;
                    var property = _buffer.Slice(propertyStart, propertyEnd - propertyStart);
                    SetToken(NameSlotTokenType.Property, property);

                }
                else if (IsOperator(_buffer[_currentPosition]))
                {
                    if (_leftToken.TokenType == NameSlotTokenType.None)
                    {
                        throw new InvalidDataException();
                    }
                    SetOperator(_buffer[_currentPosition]);
                    _currentPosition++;
                }
                else if (IsNumber(_buffer[_currentPosition]))
                {
                    if (_leftToken.TokenType == NameSlotTokenType.None)
                    {
                        throw new InvalidDataException();
                    }
                    var numberStart = _currentPosition;
                    ReadUntilEnd();
                    var numberEnd = _currentPosition;

                    SetToken(NameSlotTokenType.Number, _buffer.Slice(numberStart, numberEnd - numberStart));
                }
                _currentPosition++;
            }
        }

        private void SetToken(NameSlotTokenType tokenType, ReadOnlySpan<byte> token)
        {
            if (_leftToken.TokenType == NameSlotTokenType.None)
            {
                _leftToken = new NameSlotToken(tokenType, token);
            }
            else
            {
                _rightToken = new NameSlotToken(tokenType, token);
            }
        }

        private void SetOperator(byte ch)
        {
            switch (ch)
            {
                case NameSlotConstants.Add:
                    _operator = NameSlotOperator.Add;
                    break;
                case NameSlotConstants.Minus:
                    _operator = NameSlotOperator.Minus;
                    break;
                case NameSlotConstants.Multiply:
                    _operator = NameSlotOperator.Multiply;
                    break;
                case NameSlotConstants.Divide:
                    _operator = NameSlotOperator.Divide;
                    break;
                default:
                    break;
            }
        }

        private void ReadUntilEnd()
        {
            while (_currentPosition < _buffer.Length)
            {
                if (_buffer[_currentPosition] == NameSlotConstants.Space)
                {
                    return;
                }
                _currentPosition++;
            }
        }

        private void ReadJsonPathEnd()
        {
            while (_currentPosition < _buffer.Length)
            {
                if (_buffer[_currentPosition] == NameSlotConstants.JsonPathRoot)
                {
                    return;
                }
                _currentPosition++;
            }
            _currentPosition--;
            throw new InvalidDataException("json path must end with '$");
        }

        private bool IsNumber(byte ch)
        {
            return ch >= NameSlotConstants.Num0 && ch <= NameSlotConstants.Num9;
        }

        private bool IsOperator(byte ch)
        {
            return ch == NameSlotConstants.Add
                || _buffer[_currentPosition] == NameSlotConstants.Minus
                || _buffer[_currentPosition] == NameSlotConstants.Multiply
                || _buffer[_currentPosition] == NameSlotConstants.Divide;
        }

        private bool IsPropertyStartChar(byte ch)
        {
            return ch >= NameSlotConstants.a && ch <= NameSlotConstants.z
                || ch >= NameSlotConstants.A && ch <= NameSlotConstants.Z
                || ch == NameSlotConstants._
                || ch == NameSlotConstants.JsonPathRoot;
        }

        private bool IsPropertyChar(byte ch)
        {
            return ch >= NameSlotConstants.a && ch <= NameSlotConstants.z
                || ch >= NameSlotConstants.A && ch <= NameSlotConstants.Z
                || ch >= NameSlotConstants.Num0 && ch <= NameSlotConstants.Num9
                || ch == NameSlotConstants._;
        }

        private void SkipInvaild()
        {
            while (_currentPosition < _buffer.Length)
            {
                if (_buffer[_currentPosition] <= NameSlotConstants.Space)
                {
                    _currentPosition++;
                    continue;
                }
                break;
            }
        }

    }
}
