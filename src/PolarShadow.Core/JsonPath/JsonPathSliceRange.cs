using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public struct JsonPathSliceRange
    {
        public JsonPathSliceRange(int? start, int? end)
        {
            this.Start = start;
            this.End = end;
        }
        public int? Start { get; }
        public int? End { get; }

        public override string ToString()
        {
            return $"{Start}:{End}";
        }
    }
}
