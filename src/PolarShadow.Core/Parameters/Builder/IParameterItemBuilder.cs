using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface IParameterItemBuilder : IPolarShadowItemBuilder
    {
        IParameter Build(JsonElement parameters);
    }
}
