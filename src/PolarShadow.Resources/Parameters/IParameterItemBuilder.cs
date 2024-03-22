using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public interface IParameterItemBuilder : IPolarShadowItemBuilder
    {
        IKeyValueParameter PrefabParameters { get; set; }
    }
}
