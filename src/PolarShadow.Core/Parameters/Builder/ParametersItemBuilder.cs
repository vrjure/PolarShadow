using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class ParametersItemBuilder : IParameterItemBuilder
    {
        private readonly JsonElement _config;
        public  ParametersItemBuilder(JsonDocument doc)
        {
            if (!_config.TryGetProperty("parameters", out JsonElement value))
            {
                return;
            }
            _config = value.Clone();
        }

        public IParameter Build(JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Object)
            {
                return new KeyValueParameter();
            }
            var parameter = new KeyValueParameter();
            parameter.Add(value);
            return parameter;
        }

        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            return Build(_config);
        }
    }
}
