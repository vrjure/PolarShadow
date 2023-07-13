using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class AnalysisContentBuilder
    {
        /// <summary>
        /// 构建模板内容
        /// </summary>
        public static void BuildContent(this JsonElement template, Stream stream, IParameter content, IParameter parameter)
        {
            using var jsonWriter = new Utf8JsonWriter(stream, JsonOption.DefaultWriteOption);
            BuildContent(jsonWriter, template, content, parameter);
            jsonWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

        private static void BuildContent(Utf8JsonWriter jsonWriter, JsonElement template, IParameter content, IParameter parameter)
        {
            if (template.ValueKind == JsonValueKind.Array)
            {
                if (template.GetArrayLength() < 0)
                {
                    return;
                }
                BuildArrayContent(jsonWriter, template, content, parameter);
            }
            else if (template.ValueKind == JsonValueKind.Object)
            {
                jsonWriter.WriteStartObject();
                foreach (var item in template.EnumerateObject())
                {
                    jsonWriter.WritePropertyName(item.Name);
                    if (item.Value.ValueKind == JsonValueKind.Array)
                    {
                        BuildArrayContent(jsonWriter, item.Value, content, parameter);
                    }
                    else
                    {
                        BuildContent(jsonWriter, item.Value, content, parameter);
                    }
                }
                jsonWriter.WriteEndObject();
            }
            else if (template.ValueKind == JsonValueKind.String)
            {
                var str = template.GetString();
                str = str.Format(content);

                if (bool.TryParse(str, out bool boolean))
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
                template.WriteTo(jsonWriter);
            }
        }

        private static void BuildArrayContent(Utf8JsonWriter jsonWriter, JsonElement item, IParameter content, IParameter parameter)
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
                BuildArray(item, jsonWriter, content, parameter);
            }
            else
            {
                BuildTemplate(path, template, jsonWriter, content, parameter);
            }

            jsonWriter.WriteEndArray();
        }

        private static void BuildTemplate(JsonElement path, JsonElement template, Utf8JsonWriter jsonWriter, IParameter content, IParameter parameter)
        {
            if (!content.TryGetValue(path.GetString(), out ParameterValue pathValue))
            {
                return;
            }

            var childContent = new Parameters(parameter);
            var last = childContent.Count;

            if (pathValue.ValueKind == ParameterValueKind.Json)
            {
                var jsonPathValue = pathValue.GetJson();

                foreach (var child in jsonPathValue.EnumerateArray())
                {
                    childContent[last] = new ObjectParameter(new ParameterValue(child));
                    BuildContent(jsonWriter, template, childContent, parameter);
                }
            }
            else if (pathValue.ValueKind == ParameterValueKind.Html)
            {
                var htmlPathValue = pathValue.GetHtml();
                if (htmlPathValue.ValueKind == HtmlValueKind.Nodes)
                {
                    foreach (var child in htmlPathValue.EnumerateNodes())
                    {
                        childContent[last] = new ObjectParameter(new ParameterValue(child));
                        BuildContent(jsonWriter, template, childContent, parameter);
                    }
                }
                else if(htmlPathValue.ValueKind == HtmlValueKind.Node)
                {
                    childContent[last] = new ObjectParameter(new ParameterValue(htmlPathValue));
                    BuildContent(jsonWriter, template, childContent, parameter);
                }

            }
        }

        private static void BuildArray(JsonElement item, Utf8JsonWriter jsonWriter, IParameter content, IParameter input)
        {
            foreach (var obj in item.EnumerateArray())
            {
                BuildContent(jsonWriter, obj, content, input);
            }
        }
    }
}
