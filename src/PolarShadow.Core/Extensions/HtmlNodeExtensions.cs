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
        public static T Analysis<T>(this HtmlNode node, JsonElement param, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            return new HtmlAnalysisHandler(node, param).Analysis<T>(node, actions);
        }

        public static void Analysis(this HtmlNode node, JsonElement param, Stream stream, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            new HtmlAnalysisHandler(node, param).Analysis(node, stream, actions);
        }
    }
}
