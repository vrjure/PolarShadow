using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static partial class ParameterExtensions
    {
        public static void Add(this IKeyValueParameter provider, string name, decimal value) => provider.Add(name, new ParameterValue(value));
        public static void Add(this IKeyValueParameter provider, string name, string value) => provider.Add(name, new ParameterValue(value));
        public static void Add(this IKeyValueParameter provider, string name, bool value) => provider.Add(name, new ParameterValue(value));
        public static void Add(this IKeyValueParameter provider, string name, JsonElement value) => provider.Add(name, new ParameterValue(value));
        public static void Add(this IKeyValueParameter provider, string name, HtmlElement value) => provider.Add(name, new ParameterValue(value));
        public static void Add(this IKeyValueParameter provider, JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException("JsonValueKind must be a object");
            }

            foreach (var item in value.EnumerateObject())
            {
                switch (item.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                    case JsonValueKind.Array:
                        provider.Add(item.Name, item.Value);
                        break;
                    case JsonValueKind.String:
                        provider.Add(item.Name, item.Value.GetString());
                        break;
                    case JsonValueKind.Number:
                        provider.Add(item.Name, item.Value.GetDecimal());
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        provider.Add(item.Name, item.Value.GetBoolean());
                        break;
                    default:
                        break;
                }
            }
        }
        public static void Add<T>(this IKeyValueParameter provider, T value) where T :class
        {
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(value, JsonOption.DefaultSerializer));
            provider.Add(doc.RootElement.Clone());
        }
    }
}
