using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public partial struct NameSlotReader
    {
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
    }
}
