using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public enum JsonPathTokenType
    {
        None = 0,
        Root,
        SelectCurrentNode,
        Wildcard,
        DeepScan,
        Child,
        PropertyNameFilterStart,
        PropertyNameFilterEnd,
        ArrayIndexFilterStart,
        ArrayIndexFilterEnd,
        ArraySliceFilterStart,
        ArraySliceFilterEnd,
        ExpressionFilterStart,
        ExpressionFilterEnd,
        PropertyName,
        Number,
        Operator,
        SliceRange
    }
}
