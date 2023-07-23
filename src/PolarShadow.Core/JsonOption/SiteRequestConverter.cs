using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    public class SiteRequestConverter<T> : JsonConverter<ISiteRequest> where T : ISiteRequest
    {
        public override ISiteRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, ISiteRequest value, JsonSerializerOptions options)
        {
            value.WriteTo(writer);
        }
    }
}
