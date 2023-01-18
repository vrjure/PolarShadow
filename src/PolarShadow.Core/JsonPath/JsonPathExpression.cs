using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public ref struct JsonPathExpression
    {
        private readonly ReadOnlySpan<byte> _expression;

        public JsonPathExpression(ReadOnlySpan<byte> expression)
        {
            _expression = expression;
        }

        public void ReadLeft()
        {

        }

        public void ReadOperate()
        {

        }

        public void ReadRight()
        {

        }
    }
}
