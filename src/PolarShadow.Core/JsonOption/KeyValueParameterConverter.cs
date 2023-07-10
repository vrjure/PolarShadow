using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    public class KeyValueParameterConverter : JsonConverter<IParameter>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.GetInterface(nameof(IParameter)) != null
                || typeToConvert == typeof(IParameter);
        }
        public override IParameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var keyValueParameter = new KeyValueParameter();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return keyValueParameter;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                reader.Read();
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                        var ele = JsonElement.ParseValue(ref reader);
                        keyValueParameter.Add(propertyName, ele);
                        break;
                    case JsonTokenType.String:
                        keyValueParameter.Add(propertyName, reader.GetString());
                        break;
                    case JsonTokenType.Number:
                        keyValueParameter.Add(propertyName, reader.GetDecimal());
                        break;
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        keyValueParameter.Add(propertyName, reader.GetBoolean());
                        break;
                    case JsonTokenType.Null:
                        break;
                    default:
                        break;
                }
            }

            return keyValueParameter;
        }

        public override void Write(Utf8JsonWriter writer, IParameter value, JsonSerializerOptions options)
        {
            
        }
    }
}
