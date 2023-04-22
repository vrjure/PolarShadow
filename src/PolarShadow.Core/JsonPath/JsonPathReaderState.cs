using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public struct JsonPathReaderState
    {
        public JsonPathReaderState(int position, JsonPathTokenType tokenType)
        {
            this.Position = position;
            this.TokenType = tokenType;
        }
        internal int Position;
        internal JsonPathTokenType TokenType;
    }
}
