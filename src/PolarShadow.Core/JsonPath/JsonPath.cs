using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public ref struct JsonPath
    {
        private readonly ReadOnlySpan<byte> _jsonPath;
        public JsonPath(ReadOnlySpan<byte> jsonPath)
        {
            _jsonPath = jsonPath;
        }

        public T Read<T>(JsonElement root)
        {
            var reader = new JsonPathReader(_jsonPath);
            if(!reader.Read() || reader.TokenType != JsonPathTokenType.Root) return default(T);
            var value = ReadRootNext(ref reader, root, root);
            return default;
        }

        private JsonElement ReadRootNext(ref JsonPathReader reader, JsonElement current, JsonElement root)
        {
            if (!reader.Read())
            {
                return default;
            }

            return reader.TokenType switch
            {
                JsonPathTokenType.Child => ReadChildNext(ref reader, current, root),
                JsonPathTokenType.DeepScan => ReadDeepScanNext(ref reader, current, root),
                JsonPathTokenType.StartFilter => ReadStartFilterNext(ref reader, current, root),
                _ => default
            };
        }

        private JsonElement ReadChildNext(ref JsonPathReader reader, JsonElement current, JsonElement root)
        {
            if (!reader.Read())
            {
                return default;
            }

            var value = reader.TokenType switch
            {
                JsonPathTokenType.PropertyName => current.GetProperty(reader.Slice()),
                JsonPathTokenType.Wildcard => current,
                _ => default
            };

            if (!reader.Read()) return value;

            return ReadPropertyNext(ref reader, value, root);
        }

        private JsonElement ReadDeepScanNext(ref JsonPathReader reader, JsonElement current, JsonElement root)
        {
            if (!reader.Read())
            {
                return default;
            }

            var value = reader.TokenType switch
            {
                JsonPathTokenType.PropertyName => DeepScanProperty(current, reader.Slice()),
                JsonPathTokenType.Wildcard => current,
                _ => default
            };

            if (!reader.Read()) return value;

            return ReadPropertyNext(ref reader, value, root);
        }

        private JsonElement ReadPropertyNext(ref JsonPathReader reader, JsonElement current, JsonElement root)
        {
            if (!reader.Read()) return default;

            if (current.ValueKind == JsonValueKind.Array)
            {
                var ms = new MemoryStream();
                using var jsonWriter = new Utf8JsonWriter(ms);

                var state = reader.State;
                jsonWriter.WriteStartArray();
                foreach (var item in current.EnumerateArray())
                {
                    reader.Reset(state);
                    var value = reader.TokenType switch
                    {
                        JsonPathTokenType.Child => ReadChildNext(ref reader, item, root),
                        JsonPathTokenType.DeepScan => ReadDeepScanNext(ref reader, item, root),
                        JsonPathTokenType.StartFilter => ReadStartFilterNext(ref reader, item, root),
                        _ => default
                    };

                    if (value.ValueKind == JsonValueKind.Undefined)
                    {
                        continue;
                    }
                    value.WriteTo(jsonWriter);
                }

                jsonWriter.WriteEndArray();
                ms.Seek(0, SeekOrigin.Begin);
                using var doc = JsonDocument.Parse(ms);
                
                return doc.RootElement.Clone();
            }
            else if (current.ValueKind == JsonValueKind.Object)
            {
                return reader.TokenType switch
                {
                    JsonPathTokenType.Child => ReadChildNext(ref reader, current, root),
                    JsonPathTokenType.DeepScan => ReadDeepScanNext(ref reader, current, root),
                    JsonPathTokenType.StartFilter => ReadStartFilterNext(ref reader, current, root),
                    _ => default
                };
            }

            return default;

        }

        private JsonElement ReadStartFilterNext(ref JsonPathReader reader, JsonElement current, JsonElement root)
        {
            if (!reader.Read()) return default;

            return reader.TokenType switch
            {
                JsonPathTokenType.StartExpression => ReadStartExpressionNext(ref reader, current, root)
            };
            return default;
        }

        private JsonElement ReadStartExpressionNext(ref JsonPathReader reader, JsonElement current, JsonElement root)
        {
            if (!reader.Read()) return default;

            var left = reader.TokenType switch
            {
                JsonPathTokenType.Current => ReadCurrentNext(ref reader, current, root),
                JsonPathTokenType.Root => ReadRootNext(ref reader, root, root),
                _ => default
            };

            if (left.ValueKind == JsonValueKind.Undefined) return default;

            if (!reader.Read()) return default;
            var op = reader.TokenType;
            if (op == JsonPathTokenType.EndExpression && CaculateExpression(left, default(object), JsonPathTokenType.None))
            {
                return current;
            }

            if (!reader.Read()) return default;
            var right = reader.TokenType switch
            {
                JsonPathTokenType.Root => ReadRootNext(ref reader, root, root),
                JsonPathTokenType.Current => ReadCurrentNext(ref reader, current, root),
                _ => default
            }; ;

            if (CaculateExpression(left, right, op))
            {
                return current;
            }
            return default;
        }

        private JsonElement ReadCurrentNext(ref JsonPathReader reader, JsonElement current, JsonElement root)
        {
            if (!reader.Read()) return default;

            return reader.TokenType switch
            {
                JsonPathTokenType.Child => ReadChildNext(ref reader, current, root),
                JsonPathTokenType.DeepScan => ReadDeepScanNext(ref reader, current, root),
                _ => default
            };
        }

        private JsonElement DeepScanProperty(JsonElement current, ReadOnlySpan<byte> propertyName, Func<JsonProperty, bool> filter = default)
        {
            if (current.ValueKind == JsonValueKind.Object)
            {
                foreach (var item in current.EnumerateObject())
                {
                    if (!item.NameEquals(propertyName))
                    {
                        continue;
                    }

                    if (filter == default || filter != default && filter(item))
                    return item.Value;
                }
            }
            else if (current.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in current.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object)
                    {
                        break;
                    }
                    return DeepScanProperty(item, propertyName);
                }
            }

            return default;
        }

        private bool CaculateExpression<TLeft, TRight>(TLeft left, TRight right, JsonPathTokenType op)
        {
            if (op == JsonPathTokenType.None)
            {
                if (left is JsonElement json) return CaculateBoolean(json);

                return false;
            }

            if (left is JsonElement jsonLeft)
            {

            }

            if (right is JsonElement jsonRight)
            {

            }

            switch (op)
            {
                case JsonPathTokenType.Equal:
                    break;
                case JsonPathTokenType.LessThan:
                    break;
                case JsonPathTokenType.GreaterThan:
                    break;
                case JsonPathTokenType.LessThenOrEqual:
                    break;
                case JsonPathTokenType.GreaterThanOrEqual:
                    break;
                case JsonPathTokenType.NotEqual:
                    break;
                case JsonPathTokenType.Matches:
                    break;
                case JsonPathTokenType.In:
                    break;
                case JsonPathTokenType.Nin:
                    break;
                case JsonPathTokenType.Subsetof:
                    break;
                case JsonPathTokenType.Anyof:
                    break;
                case JsonPathTokenType.Noneof:
                    break;
                case JsonPathTokenType.Size:
                    break;
                case JsonPathTokenType.Empty:
                    break;
                default:
                    break;
            }
        }

        private bool CaculateBoolean(JsonElement json)
        {
            return json.ValueKind switch
            {
                JsonValueKind.String => string.IsNullOrEmpty(json.GetString()),
                JsonValueKind.Number => !(json.GetDecimal() == 0),
                JsonValueKind.False => false,
                JsonValueKind.True => true,
                _ => false
            };
        }
    }
}
