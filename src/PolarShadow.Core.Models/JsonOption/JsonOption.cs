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

        public static void Default(JsonSerializerOptions option)
        {
            option.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            option.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            option.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            option.Converters.Add(new EnumStringConverter());
        }

        public static void ForDash(JsonSerializerOptions option)
        {
            option.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            option.PropertyNamingPolicy = new JsonCamelCaseNamingPolicyDash();
            option.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            option.Converters.Add(new EnumStringConverter());
        }
    }
}
