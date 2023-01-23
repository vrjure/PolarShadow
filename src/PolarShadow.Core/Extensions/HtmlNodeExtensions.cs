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
        private static HtmlAnalysisHandler _htmHandler = new HtmlAnalysisHandler();
        public static T Analysis<T>(this HtmlNode node, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            return _htmHandler.Analysis<T>(node, actions);
        }

        public static void Analysis(this HtmlNode node, Stream stream, IReadOnlyDictionary<string, AnalysisAction> actions)
        {
            _htmHandler.Analysis(node, stream, actions);
        }
    }
}
