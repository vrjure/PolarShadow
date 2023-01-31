using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class JsonExtensions
    {
        public static void Append (this JsonElement element, JsonElement append, Stream output)
        {
            if (element.ValueKind != JsonValueKind.Object || element.ValueKind != JsonValueKind.Object)
            {
                return;
            }
            using var jsonWriter = new Utf8JsonWriter(output, JsonOption.DefaultWriteOption);
            jsonWriter.WriteStartObject();
            foreach (var item in element.EnumerateObject())
            {
                item.WriteTo(jsonWriter);
            }

            foreach (var item in append.EnumerateObject())
            {
                item.WriteTo(jsonWriter);
            }
            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
            output.Seek(0, SeekOrigin.Begin);
        }
        public static JsonElement Append(this JsonElement element, JsonElement append)
        {
            using var ms = new MemoryStream();
            Append(element, append, ms);
            using var doc = JsonDocument.Parse(ms);
            return doc.RootElement.Clone();
        }
    }
}
