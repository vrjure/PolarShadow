using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    internal class ParameterItemBuilder : IParameterItemBuilder
    {
        public IPolarShadowItem Build(string name, IPolarShadowBuilder builder)
        {
            return new ParameterItem(name);
        }
    }
}
