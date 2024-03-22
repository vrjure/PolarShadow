using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public interface IParameterItem : IPolarShadowItem
    {
        IKeyValueParameter Parameters { get; }
    }
}
