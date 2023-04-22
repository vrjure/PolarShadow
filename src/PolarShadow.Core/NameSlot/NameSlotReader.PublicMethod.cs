using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal partial struct NameSlotReader
    {
        public string GetString()
        {
            return Encoding.UTF8.GetString(Slice());
        }

        public ReadOnlySpan<byte> GetSegment()
        {
            return Slice();
        }

        private ReadOnlySpan<byte> Slice()
        {
            return _buffer.Slice(_segmentStart, _segmentEnd - _segmentStart + 1);
        }
    }
}
