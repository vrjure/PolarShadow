using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Configurations
{
    internal class PolarShadowJsonConfigurationProvider : JsonConfigurationProvider
    {
        public PolarShadowJsonConfigurationProvider(JsonConfigurationSource source) : base(source)
        {
        }
    }
}
