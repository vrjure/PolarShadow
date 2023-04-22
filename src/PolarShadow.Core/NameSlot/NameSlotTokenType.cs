using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal enum NameSlotTokenType
    {
        None = 0,
        /// <summary>
        /// {
        /// </summary>
        Start,
        /// <summary>
        /// }
        /// </summary>
        End,
        /// <summary>
        /// ::
        /// </summary>
        Format,
        /// <summary>
        /// 参数 [_][A-Z a-z]
        /// </summary>
        Parameter,
        /// <summary>
        /// :@[_][A-Z a-z]
        /// </summary>
        Property,
        /// <summary>
        /// jsonPath
        /// </summary>
        jsonPath,
        /// <summary>
        /// xPath
        /// </summary>
        xPath,
        /// <summary>
        /// 0-9 .
        /// </summary>
        Number,
        /// <summary>
        /// /.../
        /// </summary>
        Regex,
        /// <summary>
        /// ? :
        /// </summary>
        ConditionalExpression
    }
}
