using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    public class LinkConverter : JsonConverter<ILink>
    {
        public override ILink Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<Link>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, ILink value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value as Link, options);
        }
    }
}
