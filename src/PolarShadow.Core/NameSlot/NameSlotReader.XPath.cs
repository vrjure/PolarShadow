using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal partial struct NameSlotReader
    {
        private bool ReadToXPathEnd()
        {
            _index++;
            _segmentStart = _segmentEnd = _index;
            if (XPathSimpleReader.TryReadToEnd(_buffer.Slice(_segmentStart), out int consume))
            {
                _segmentEnd = _segmentStart + consume;
                return true;
            }
            return false;
        }
    }
}
