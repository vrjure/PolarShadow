using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public enum JsonPathExpressionOperator
    {
        None = 0,
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Regex,
        In,
        NIn,
        SubsetOf,
        AnyOf,
        NoneOf,
        Size,
        Empty
    }
}
