using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PolarShadow.Core
{
    public abstract class AnalysisActionHandler<TInput> : IAnalysisHandler<TInput>
    {
        public void Analysis(TInput obj, Stream stream, IReadOnlyDictionary<string, AnalysisAction> actions)
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

        public T Analysis<T>(TInput obj, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            using var ms = new MemoryStream();

            Analysis(obj, ms, actions);

            ms.Seek(0, SeekOrigin.Begin);

            return JsonSerializer.Deserialize<T>(ms, JsonOption.DefaultSerializer);
        }

        protected abstract bool VerifyInput(TInput obj);

        private void WriteJson(TInput obj, Utf8JsonWriter jsonWriter, KeyValuePair<string, AnalysisAction> action)
        {
            switch (action.Value.PathValueType)
            {
                case PathValueType.Array:
                    if (action.Value.AnalysisItem == null || action.Value.AnalysisItem.Count == 0)
                    {
                        break;
                    }

                    var nodes = HandleArray(obj, action.Value);
                    if (nodes == null)
                    {   
                        break;
                    }
                    jsonWriter.WriteStartArray(action.Key);
                    foreach (var item in nodes)
                    {
                        WriteJsonArray(item, jsonWriter, action.Value.AnalysisItem);
                    }
                    jsonWriter.WriteEndArray();
                    break;
                case PathValueType.Next:
                    if (action.Value.Next == null)
                    {
                        break;
                    }
                    var nextNode = HandleNext(obj, action.Value);
                    if (!VerifyInput(nextNode))
                    {
                        break;
                    }
                    WriteJson(nextNode, jsonWriter, new KeyValuePair<string, AnalysisAction>(action.Key, action.Value.Next));
                    break;
                case PathValueType.Attribute:
                    if (string.IsNullOrEmpty(action.Value.AttributeName))
                    {
                        break;
                    }

                    var attr = HandleAttribute(obj, action.Value);  
                    if (string.IsNullOrEmpty(attr))
                    {
                        break;
                    }

                    HandleValue(jsonWriter, attr, action);
                    break;
                case PathValueType.InnerText:
                    var innerText = HandleInnerText(obj, action.Value);
                    if (string.IsNullOrEmpty(innerText))
                    {
                        break;
                    }
                    HandleValue(jsonWriter, innerText, action);
                    break;
                case PathValueType.None:
                    jsonWriter.WriteString(action.Key, action.Value.Path);
                    break;
                case PathValueType.String:
                    var str = HandleString(obj, action.Value);
                    if (string.IsNullOrEmpty(str))
                    {
                        break;
                    }
                    HandleValue(jsonWriter, str, action);
                    break;
                case PathValueType.Number:
                    var num = HandleNumber(obj, action.Value);
                    if (num == default)
                    {
                        break;
                    }
                    HandleValue(jsonWriter, num.Value, action);
                    break;
                case PathValueType.Boolean:
                    var val = HandleBoolean(obj, action.Value);
                    if (!val.HasValue)
                    {
                        break;
                    }
                    HandleValue(jsonWriter, val.Value, action);
                    break;
                case PathValueType.Raw:
                    var raw = HandleRaw(obj, action.Value);
                    if (string.IsNullOrEmpty(raw))
                    {
                        break;
                    }
                    HandleRawValue(jsonWriter, raw, action);
                    break;
                default:
                    break;
            }
        }

        private void WriteJsonArray(TInput obj, Utf8JsonWriter jsonWriter, IReadOnlyDictionary<string, AnalysisAction> actions)
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

        private void HandleValue(Utf8JsonWriter jsonWriter, string input, KeyValuePair<string, AnalysisAction> action)
        {
            if (!string.IsNullOrEmpty(action.Value.Regex))
            {
                var macth = Regex.Match(input, action.Value.Regex);
                if (macth.Success)
                {
                    input = macth.Value;             
                }
            }

            if (!string.IsNullOrEmpty(action.Value.Format))
            {
                input = string.Format(action.Value.Format, input);
            }

            jsonWriter.WriteString(action.Key, input);

        }

        private void HandleRawValue(Utf8JsonWriter jsonWriter, string raw, KeyValuePair<string, AnalysisAction> action)
        {
            jsonWriter.WritePropertyName(action.Key);
            jsonWriter.WriteRawValue(raw);
        }

        private void HandleValue(Utf8JsonWriter jsonWriter, decimal value, KeyValuePair<string, AnalysisAction> action)
        {
            jsonWriter.WriteNumber(action.Key, value);
        }

        private void HandleValue(Utf8JsonWriter jsonWriter, bool value, KeyValuePair<string, AnalysisAction> action)
        {
            jsonWriter.WriteBoolean(action.Key, value);
        }

        protected virtual string HandleInnerText(TInput obj, AnalysisAction action)
        {
            return string.Empty;
        }

        protected virtual string HandleAttribute(TInput obj, AnalysisAction action)
        {
            return string.Empty;
        }

        protected virtual TInput HandleNext(TInput obj, AnalysisAction action)
        {
            return default;
        }

        protected virtual IEnumerable<TInput> HandleArray(TInput obj, AnalysisAction action)
        {
            return default;
        }

        protected virtual string HandleString(TInput obj, AnalysisAction action)
        {
            return string.Empty;
        }

        protected virtual decimal? HandleNumber(TInput obj, AnalysisAction action)
        {
            return default;
        }

        protected virtual bool? HandleBoolean(TInput obj, AnalysisAction action)
        {
            return default;
        }

        protected virtual string HandleRaw(TInput obj, AnalysisAction action)
        {
            return string.Empty;
        }
    }
}
