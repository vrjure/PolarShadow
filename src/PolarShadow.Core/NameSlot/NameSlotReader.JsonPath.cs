using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal partial struct NameSlotReader
    {
        private bool ReadToJsonPathEnd()
        {
            _segmentStart = _index;
            var reader = new JsonPathReader(_buffer, _segmentStart);
            var consume = reader.ReadToEnd();
            _index += consume;
            _segmentEnd = _index;
            return true;
        }
    }
}
