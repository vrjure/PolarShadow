using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public sealed class JsonOption
    {
        static JsonOption()
        {
            DefaultSerializer = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Converters = { new EnumStringConverter() }
            };

            ForDashSerializer = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = new JsonCamelCaseNamingPolicyDash(),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Converters = { new EnumStringConverter() }
            };
        }

        public static JsonSerializerOptions DefaultSerializer { get; }

        public static JsonSerializerOptions ForDashSerializer { get; }
    }
}
