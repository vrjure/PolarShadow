using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    internal class EnumStringConverter : JsonConverter<Enum>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override Enum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (Enum)Enum.Parse(typeToConvert, reader.GetInt64().ToString());
            }
            return (Enum)Enum.Parse(typeToConvert, reader.GetString() ?? "", true);
        }

        public override void Write(Utf8JsonWriter writer, Enum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
