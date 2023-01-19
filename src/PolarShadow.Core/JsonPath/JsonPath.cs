using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class JsonPath
    {
        public static T Analysis<T>(this JsonElement obj, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            using var ms = new MemoryStream();
            Analysis(obj, ms, actions);
            ms.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.Deserialize<T>(ms, JsonOption.DefaultSerializer);
        }

        public static void Analysis(this JsonElement obj, Stream stream, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            using var jsonWriter = new Utf8JsonWriter(stream, JsonOption.DefaultWriteOption);

            jsonWriter.WriteStartObject();
            foreach (var action in actions)
            {
                WriteJson(obj, jsonWriter, action);
            }
            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
        }

        private static void WriteJson(JsonElement obj, Utf8JsonWriter jsonWriter, KeyValuePair<string, AnalysisAction> action)
        {
            if (string.IsNullOrEmpty(action.Value.Path))
            {
                return;
            }

            if (action.Value.PathValueType == PathValueType.None)
            {
                jsonWriter.WriteString(action.Key, action.Value.Path);
            }

            if (TryGetPropertyWithJsonPath(obj, action.Value.Path, out JsonElement element))
            {
                switch (action.Value.PathValueType)
                {
                    case PathValueType.String:
                        jsonWriter.WriteString(action.Key, element.GetString());
                        break;
                    case PathValueType.Number:
                        jsonWriter.WriteNumber(action.Key, element.GetDecimal());
                        break;
                    case PathValueType.Boolean:
                        jsonWriter.WriteBoolean(action.Key, element.GetBoolean());
                        break;
                    case PathValueType.Array:
                        if (action.Value.AnalysisItem == null || action.Value.AnalysisItem.Count == 0)
                        {
                            break;
                        }
                        foreach (var item in element.EnumerateArray())
                        {
                            WriteJsonArray(item, jsonWriter, action.Value.AnalysisItem);
                        }
                        break;
                    case PathValueType.Object:
                        WriteJson(element, jsonWriter, new KeyValuePair<string, AnalysisAction>(action.Key, action.Value));
                        break;
                    default:
                        break;
                }
            }
        }

        private static void WriteJsonArray(JsonElement obj, Utf8JsonWriter jsonWriter, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            if (actions == null || actions.Count == 0)
            {
                return;
            }

            jsonWriter.WriteStartObject();
            foreach (var item in actions)
            {
                WriteJson(obj, jsonWriter, item);
            }
            jsonWriter.WriteEndObject();
        }

        public static bool TryGetPropertyWithJsonPath(this JsonElement obj, string jsonPath, out JsonElement result)
        {
            if (!jsonPath.TrimStart().StartsWith('$'))
            {
                return obj.TryGetProperty(jsonPath, out result);
            }

            var reader = new JsonPathReader(Encoding.UTF8.GetBytes(jsonPath));
            var lastTokenType = JsonPathTokenType.None;
            result = obj;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonPathTokenType.None:
                        break;
                    case JsonPathTokenType.Root:
                        result = obj;
                        break;
                    case JsonPathTokenType.PropertyName:
                        if (reader.TryReadProperty(out string property))
                        {
                            if (lastTokenType == JsonPathTokenType.Child)
                            {
                                if (!result.TryGetProperty(property, out result))
                                {
                                    return false;
                                }
                            }
                            else if (lastTokenType == JsonPathTokenType.DeepScan)
                            {
                                if (!TryDeepScanProperty(result, property, out result))
                                {
                                    return false;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

                lastTokenType = reader.TokenType;
            }

            return true;
        }

        private static bool TryDeepScanProperty(JsonElement root, string propertyName, out JsonElement element)
        {
            element = default;
            if (root.ValueKind != JsonValueKind.Array && root.TryGetProperty(propertyName, out element))
            {
                return true;
            }

            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var jsonProperty in root.EnumerateObject())
                {
                    if (!jsonProperty.NameEquals(propertyName))
                    {
                        continue;
                    }
                    element = jsonProperty.Value;
                    return true;
                }

                foreach (var jsonProperty in root.EnumerateObject())
                {
                    if (jsonProperty.Value.ValueKind == JsonValueKind.Object 
                        || jsonProperty.Value.ValueKind == JsonValueKind.Array)
                    {
                        if (!TryDeepScanProperty(jsonProperty.Value, propertyName, out element))
                        {
                            continue;
                        }
                        return true;
                    }
                }
            }
            else if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var arrayItem in root.EnumerateArray())
                {
                    if (arrayItem.ValueKind != JsonValueKind.Object)
                    {
                        break;
                    }

                    if (!TryDeepScanProperty(arrayItem, propertyName, out element))
                    {
                        break;
                    }

                    return true;
                }
            }


            return false;
        }
    }
}
