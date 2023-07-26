using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public class ContentBuilder : IContentBuilder
    {
        protected virtual void AfterWriteStartObject(Utf8JsonWriter writer, string propertyName) { }
        protected virtual void BeforeWriteEndObject(Utf8JsonWriter writer, string propertyName) { }
        protected virtual void AfterWriteStartArray(Utf8JsonWriter writer, string propertyName) { }
        protected virtual void BeforeWriteEndArray(Utf8JsonWriter writer, string propertyName) { }

        public virtual void BuildContent(Utf8JsonWriter writer, JsonElement template, IParameter parameter)
        {
            switch (template.ValueKind)
            {
                case JsonValueKind.Object:
                    BuildObject(writer, template, parameter);
                    break;
                case JsonValueKind.Array:
                    BuildArray(writer, template, parameter);
                    break;
                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                    BuildValue(writer, template, parameter);
                    break;
                default:
                    break;
            }
        }

        private void BuildObject(Utf8JsonWriter writer, JsonElement template, IParameter parameter, string propertyName = default)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                writer.WriteStartObject();
            }
            else
            {
                writer.WriteStartObject(propertyName);
            }

            AfterWriteStartObject(writer, propertyName);

            foreach (var property in template.EnumerateObject())
            {
                switch (property.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        BuildObject(writer, property.Value, parameter, property.Name);
                        break;
                    case JsonValueKind.Array:
                        BuildArray(writer, property.Value, parameter, property.Name);
                        break;
                    case JsonValueKind.String:
                    case JsonValueKind.Number:
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        BuildValue(writer, property, parameter);
                        break;
                    default:
                        break;
                }
            }

            BeforeWriteEndObject(writer, propertyName);

            writer.WriteEndObject();
        }

        private void BuildArray(Utf8JsonWriter writer, JsonElement tempalte, IParameter parameter, string propertyName = default)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                writer.WriteStartArray();
            }
            else
            {
                writer.WriteStartArray(propertyName);
            }

            AfterWriteStartArray(writer, propertyName);

            JsonElement templateObj = default;
            foreach (var obj in tempalte.EnumerateArray())
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
                foreach (var obj in tempalte.EnumerateArray())
                {
                    BuildContent(writer, obj, parameter);
                }
            }
            else
            {
                BuildArrayTemplate(writer, path, template, parameter);
            }

            BeforeWriteEndArray(writer, propertyName);

            writer.WriteEndArray();
        }

        private void BuildValue(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            writer.WritePropertyName(property.Name);
            BuildValue(writer, property.Value, parameter);
        }

        private void BuildValue(Utf8JsonWriter writer, JsonElement value, IParameter parameter)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.String:
                    var str = value.GetString();
                    str = str.Format(parameter);
                    if (bool.TryParse(str, out bool boolean))
                    {
                        writer.WriteBooleanValue(boolean);
                    }
                    else
                    {
                        writer.WriteStringValue(str);
                    }
                    break;
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                    value.WriteTo(writer);
                    break;
                default:
                    break;
            }
        }

        private void BuildArrayTemplate(Utf8JsonWriter jsonWriter, JsonElement path, JsonElement template, IParameter parameter)
        {
            if (!parameter.TryGetValue(path.GetString(), out ParameterValue pathValue))
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
                    childContent[last - 1] = new ObjectParameter(new ParameterValue(child));
                    BuildContent(jsonWriter, template, parameter);
                }
            }
            else if (pathValue.ValueKind == ParameterValueKind.Html)
            {
                var htmlPathValue = pathValue.GetHtml();
                if (htmlPathValue.ValueKind == HtmlValueKind.Nodes)
                {
                    foreach (var child in htmlPathValue.EnumerateNodes())
                    {
                        childContent[last - 1] = new ObjectParameter(new ParameterValue(child));
                        BuildContent(jsonWriter, template, parameter);
                    }
                }
                else if (htmlPathValue.ValueKind == HtmlValueKind.Node)
                {
                    childContent[last - 1] = new ObjectParameter(new ParameterValue(htmlPathValue));
                    BuildContent(jsonWriter, template, parameter);
                }

            }
        }
    }
}
