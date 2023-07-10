using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static partial class ParameterExtensions
    {
        public static void Add(this IObjectParameter provider, JsonElement value) => provider.Add(new ParameterValue(value));

        public static void Add(this IObjectParameter provider, HtmlElement value) => provider.Add(new ParameterValue(value));

        public static void Add<T>(this IObjectParameter provider, T value) where T : class
        {
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(value, JsonOption.DefaultSerializer));
            provider.Add(new ParameterValue(doc.RootElement.Clone()));
        }
    }
}
