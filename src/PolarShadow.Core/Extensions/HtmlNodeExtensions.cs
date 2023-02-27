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
        private readonly static HtmlAnalysisHandler _handler = new HtmlAnalysisHandler();
        public static T Analysis<T>(this HtmlNode node, JsonElement param, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            return _handler.Analysis<T>(node, actions, param);
        }

        public static void Analysis(this HtmlNode node, JsonElement param, Stream stream, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            _handler.Analysis(node, stream, actions, param);
        }
    }
}
