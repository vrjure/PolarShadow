using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PolarShadow.Core
{
    public abstract class AnalysisHandler<TInput> : IAnalysisHandler<TInput>
    {
        T IAnalysisHandler.Analysis<T>(object obj, IReadOnlyDictionary<string, AnalysisAction> actions) where T : class
        {
            return Analysis<T>((TInput)obj, actions);
            
        }

        public T Analysis<T>(TInput obj, IReadOnlyDictionary<string, AnalysisAction> actions) where T : class
        {
            using var ms = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(ms, JsonOption.DefaultWriteOption);
            jsonWriter.WriteStartObject();
            foreach (var action in actions)
            {
                WriteJson(obj, jsonWriter, action);
            }
            jsonWriter.WriteEndObject();
            jsonWriter.Flush();

            ms.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.Deserialize<T>(ms, JsonOption.DefaultSerializer);
        }

        protected abstract bool VerifyInput(TInput obj);

        protected virtual void WriteJson(TInput obj, Utf8JsonWriter jsonWriter, KeyValuePair<string, AnalysisAction> action)
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
                case PathValueType.Object:
                    if (action.Value.Next == null)
                    {
                        break;
                    }
                    var nextNode = HandleObject(obj, action.Value);
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
            if (string.IsNullOrEmpty(action.Value.Regex))
            {
                jsonWriter.WriteString(action.Key, input);
            }
            else
            {
                var macth = Regex.Match(input, action.Value.Regex);
                if (macth.Success)
                {
                    jsonWriter.WriteString(action.Key, macth.Value);
                }
            }
        }

        protected virtual string HandleInnerText(TInput obj, AnalysisAction action)
        {
            return string.Empty;
        }

        protected virtual string HandleAttribute(TInput obj, AnalysisAction action)
        {
            return string.Empty;
        }

        protected virtual TInput HandleObject(TInput obj, AnalysisAction action)
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
    }
}
