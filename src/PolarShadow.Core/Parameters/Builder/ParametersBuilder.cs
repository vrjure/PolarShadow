using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal sealed class ParametersBuilder
    {
        public static IParameter Build(JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Object)
            {
                return new KeyValueParameter();
            }
            var parameter = new KeyValueParameter();
            parameter.Add(value);
            return parameter;
        }
    }
}
