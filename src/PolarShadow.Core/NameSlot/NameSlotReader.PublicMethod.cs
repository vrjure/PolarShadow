using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal partial struct NameSlotReader
    {
        public string GetString()
        {
            if (_tokenType == NameSlotTokenType.Property)
            {
                return Encoding.UTF8.GetString(Slice());
            }

            return string.Empty;
        }

        public ReadOnlySpan<byte> GetSegment()
        {
            if (_tokenType == NameSlotTokenType.jsonPath)
            {
                return Slice();
            }
            return default;
        }

        private ReadOnlySpan<byte> Slice()
        {
            return _buffer.Slice(_segmentStart, _segmentEnd - _segmentStart + 1);
        }
    }
}
