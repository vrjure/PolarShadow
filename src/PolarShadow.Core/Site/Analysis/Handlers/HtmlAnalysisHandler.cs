using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal class HtmlAnalysisHandler : AnalysisActionHandler<HtmlNode>
    {
        protected override bool VerifyInput(HtmlNode obj)
        {
            return obj != null;
        }
        protected override string HandleInnerText(HtmlNode obj, AnalysisAction action)
        {
            var node = obj.SelectSingleNode(action.Path);
            if (node == null)
            {
                return string.Empty;
            }

            return node.InnerText;
        }

        protected override string HandleAttribute(HtmlNode obj, AnalysisAction action)
        {
            var node = obj.SelectSingleNode(action.Path);
            if (node == null)
            {
                return string.Empty;
            }

            return node.GetAttributeValue(action.AttributeName, "");
        }

        protected override HtmlNode HandleObject(HtmlNode obj, AnalysisAction action)
        {
            return obj.SelectSingleNode(action.Path);
        }

        protected override IEnumerable<HtmlNode> HandleArray(HtmlNode obj, AnalysisAction action)
        {
            return obj.SelectNodes(action.Path);
        }
    }
}
