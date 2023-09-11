using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PolarShadow.Aria2
{
    internal static class JsonOption
    {
        static JsonOption()
        {

            ForDashSerializer = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = new JsonCamelCaseNamingPolicyDash(),
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };           
        }

        public static JsonSerializerOptions ForDashSerializer { get; }
        
    }
}
