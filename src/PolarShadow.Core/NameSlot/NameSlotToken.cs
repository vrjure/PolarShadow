using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal ref struct NameSlotToken
    {
        public NameSlotToken(NameSlotTokenType tokenType, ReadOnlySpan<byte> token)
        {
            this.TokenType = tokenType;
            this.Token = token;
        }
        public NameSlotTokenType TokenType { get; }
        public ReadOnlySpan<byte> Token { get; }
    }
}
