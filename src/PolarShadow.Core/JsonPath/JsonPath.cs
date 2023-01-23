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
        private static JsonAnalysisHandler _jsonHandler = new JsonAnalysisHandler();

        public static T Analysis<T>(this JsonElement obj, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            return _jsonHandler.Analysis<T>(obj, actions);
        }

        public static void Analysis(this JsonElement obj, Stream stream, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            _jsonHandler.Analysis(obj, stream, actions);
        }

        public static bool TryGetPropertyWithJsonPath(this JsonElement obj, ReadOnlySpan<byte> jsonPath, out JsonElement result)
        {
            if (jsonPath.Length == 0)
            {
                result = new JsonElement();
                return false;
            }
            if (!jsonPath[0].Equals(JsonPathConstants.Root))
            {
                return obj.TryGetProperty(jsonPath, out result);
            }

            var reader = new JsonPathReader(jsonPath);
            var lastTokenType = JsonPathTokenType.None;
            result = new JsonElement();
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

        public static bool TryGetPropertyWithJsonPath(this JsonElement obj, string jsonPath, out JsonElement result)
        {
            return TryGetPropertyWithJsonPath(obj, Encoding.UTF8.GetBytes(jsonPath), out result);
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
