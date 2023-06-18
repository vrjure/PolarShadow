using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class AnalysisContentBuilder
    {
        public static void BuildContent(this JsonElement template, Stream stream, NameSlotValueCollection input)
        {
            using var jsonWriter = new Utf8JsonWriter(stream, JsonOption.DefaultWriteOption);
            BuildContent(jsonWriter, template, input);
            jsonWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

        private static void BuildContent(Utf8JsonWriter jsonWriter, JsonElement body, NameSlotValueCollection input)
        {
            if (body.ValueKind == JsonValueKind.Array)
            {
                if (body.GetArrayLength() < 0)
                {
                    return;
                }
                BuildArrayContent(jsonWriter, body, input);
            }
            else if (body.ValueKind == JsonValueKind.Object)
            {
                jsonWriter.WriteStartObject();
                foreach (var item in body.EnumerateObject())
                {
                    jsonWriter.WritePropertyName(item.Name);
                    if (item.Value.ValueKind == JsonValueKind.Array || item.Value.ValueKind == JsonValueKind.Object)
                    {
                        BuildArrayContent(jsonWriter, item.Value, input);
                    }
                    else
                    {
                        BuildContent(jsonWriter, item.Value, input);
                    }
                }
                jsonWriter.WriteEndObject();
            }
            else if (body.ValueKind == JsonValueKind.String)
            {
                var str = body.GetString();
                str = str.Format(input);

                if (decimal.TryParse(str, out decimal num))
                {
                    jsonWriter.WriteNumberValue(num);
                }
                else if (bool.TryParse(str, out bool boolean))
                {
                    jsonWriter.WriteBooleanValue(boolean);
                }
                else
                {
                    jsonWriter.WriteStringValue(str);
                }
            }
            else
            {
                body.WriteTo(jsonWriter);
            }
        }

        private static void BuildArrayContent(Utf8JsonWriter jsonWriter, JsonElement item, NameSlotValueCollection input)
        {
            if (item.ValueKind != JsonValueKind.Array)
            {
                return;
            }
            jsonWriter.WriteStartArray();

            JsonElement templateObj = default;
            foreach (var obj in item.EnumerateArray())
            {
                if (obj.ValueKind == JsonValueKind.Object)
                {
                    templateObj = obj;
                    break;
                }
            }

            if (templateObj.ValueKind != JsonValueKind.Object
                || !templateObj.TryGetProperty("path", out JsonElement path)
                || !templateObj.TryGetProperty("template", out JsonElement template)
                || path.ValueKind != JsonValueKind.String
                || template.ValueKind != JsonValueKind.Object)
            {
                BuildArray(item, jsonWriter, input);
            }
            else
            {
                BuildTemplate(path, template, jsonWriter, input);
            }

            jsonWriter.WriteEndArray();
        }

        private static void BuildTemplate(JsonElement path, JsonElement template, Utf8JsonWriter jsonWriter, NameSlotValueCollection input)
        {
            if (!input.TryReadValue(path.GetString(), out NameSlotValue pathValue))
            {
                return;
            }

            if (pathValue.ValueKind == NameSlotValueKind.Json)
            {
                var jsonPathValue = pathValue.GetJson();
                foreach (var child in jsonPathValue.EnumerateArray())
                {
                    var childInput = input.Clone();
                    childInput.Add(child);
                    BuildContent(jsonWriter, template, childInput);
                }
            }
            else if (pathValue.ValueKind == NameSlotValueKind.Html)
            {
                var htmlPathValue = pathValue.GetHtml();
                if (htmlPathValue.ValueKind == HtmlValueKind.Nodes)
                {
                    foreach (var child in htmlPathValue.EnumerateNodes())
                    {
                        var childInput = input.Clone();
                        childInput.Add(child);
                        BuildContent(jsonWriter, template, childInput);
                    }
                }
                else if(htmlPathValue.ValueKind == HtmlValueKind.Node)
                {
                    var childInput = input.Clone();
                    childInput.Add(htmlPathValue);
                    BuildContent(jsonWriter, template, childInput);
                }

            }
        }

        private static void BuildArray(JsonElement item, Utf8JsonWriter jsonWriter, NameSlotValueCollection input)
        {
            foreach (var obj in item.EnumerateArray())
            {
                BuildContent(jsonWriter, obj, input);
            }
        }
    }
}
