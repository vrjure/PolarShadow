using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    internal class ParameterItemBuilder : IPolarShadowItemBuilder
    {
        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            return new ParameterItem();
        }
    }
}
