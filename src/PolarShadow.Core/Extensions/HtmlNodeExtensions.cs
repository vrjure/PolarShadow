using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PolarShadow.Core
{
    public static class HtmlNodeExtensions
    {
        public static T Analysis<T>(this HtmlNode node, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            using var ms = new MemoryStream();
            Analysis(node, ms, actions);
            ms.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.Deserialize<T>(ms, JsonOption.DefaultSerializer);
        }

        public static void Analysis(this HtmlNode node, Stream stream, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            using var jsonWriter = new Utf8JsonWriter(stream, JsonOption.DefaultWriteOption);

            jsonWriter.WriteStartObject();
            foreach (var action in actions)
            {
                WriteJson(node, jsonWriter, action);
            }
            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
        }

        private static void WriteJsonArray(HtmlNode node, Utf8JsonWriter jsonWriter, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            if (actions == null || actions.Count == 0)
            {
                return;
            }
            jsonWriter.WriteStartObject();
            foreach (var item in actions)
            {
                WriteJson(node, jsonWriter, item);
            }
            jsonWriter.WriteEndObject();

        }

        private static void WriteJson(HtmlNode node, Utf8JsonWriter jsonWriter, KeyValuePair<string, AnalysisAction> action)
        {
            if (string.IsNullOrEmpty(action.Value.Path))
            {
                return;
            }

            switch (action.Value.PathValueType)
            {
                case PathValueType.Array:
                    if (action.Value.AnalysisItem == null || action.Value.AnalysisItem.Count == 0)
                    {
                        break;
                    }
                    var nodes = node.SelectNodes(action.Value.Path);
                    if (nodes == null || nodes.Count == 0)
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
                    var nextNode = node.SelectSingleNode(action.Value.Path);
                    if (nextNode == null)
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

                    var attrNode = node.SelectSingleNode(action.Value.Path);
                    if (attrNode == null)
                    {
                        break;
                    }
                    var attr = node.GetAttributeValue(action.Value.AttributeName, null);
                    if (attr == null)
                    {
                        break;
                    }

                    HandleValue(jsonWriter, attr, action);
                    break;
                case PathValueType.InnerText:
                    var innerTextNode = node.SelectSingleNode(action.Value.Path);
                    if (innerTextNode == null)
                    {
                        break;
                    }
                    if (string.IsNullOrEmpty(innerTextNode.InnerText))
                    {
                        break;
                    }

                    HandleValue(jsonWriter, innerTextNode.InnerText, action);
                    break;
                case PathValueType.None:
                    jsonWriter.WriteString(action.Key, action.Value.Path);
                    break;
                default:
                    break;
            }
        }

        private static void HandleValue(Utf8JsonWriter jsonWriter, string input, KeyValuePair<string, AnalysisAction> action)
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
    }
}
