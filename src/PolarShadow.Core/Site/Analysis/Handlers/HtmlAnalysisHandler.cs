using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class HtmlAnalysisHandler : AnalysisActionHandler<HtmlNode>
    {
        protected override bool VerifyInput(HtmlNode obj)
        {
            return obj != null;
        }
        protected override string HandleInnerText(HtmlNode obj, AnalysisAction action, JsonElement param)
        {
            var node = obj.SelectSingleNode(action.Path);
            if (node == null)
            {
                return string.Empty;
            }

            return node.InnerText;
        }

        protected override string HandleAttribute(HtmlNode obj, AnalysisAction action, JsonElement param)
        {
            var node = obj.SelectSingleNode(action.Path);
            if (node == null)
            {
                return string.Empty;
            }

            return node.GetAttributeValue(action.AttributeName, "");
        }

        protected override HtmlNode HandleNext(HtmlNode obj, AnalysisAction action, JsonElement param)
        {
            return obj.SelectSingleNode(action.Path);
        }

        protected override IEnumerable<HtmlNode> HandleArray(HtmlNode obj, AnalysisAction action, JsonElement param)
        {
            return obj.SelectNodes(action.Path);
        }

        protected override string HandleString(HtmlNode obj, AnalysisAction action, JsonElement param)
        {
            if (param.ValueKind != JsonValueKind.Undefined)
            {
                var path = action.Path.NameSlot(param);
                if (param.TryGetPropertyWithJsonPath(path, out JsonElement result))
                {
                    return result.GetString();
                }
            }

            return base.HandleString(obj, action, param);

        }

        protected override bool? HandleBoolean(HtmlNode obj, AnalysisAction action, JsonElement param)
        {
            if (param.ValueKind != JsonValueKind.Undefined)
            {
                var path = action.Path.NameSlot(param);
                if (param.TryGetPropertyWithJsonPath(path, out JsonElement result))
                {
                    return result.GetBoolean();
                }
            }
            return base.HandleBoolean(obj, action, param);
        }

        protected override decimal? HandleNumber(HtmlNode obj, AnalysisAction action, JsonElement param)
        {
            if (param.ValueKind != JsonValueKind.Undefined)
            {
                var path = action.Path.NameSlot(param);
                if (param.TryGetPropertyWithJsonPath(path, out JsonElement result))
                {
                    return result.GetDecimal();
                }
            }
            return base.HandleNumber(obj, action, param);
        }

        protected override string HandleRaw(HtmlNode obj, AnalysisAction action, JsonElement param)
        {
            if (param.ValueKind != JsonValueKind.Undefined)
            {
                var path = action.Path.NameSlot(param);
                if (param.TryGetPropertyWithJsonPath(path, out JsonElement result))
                {
                    return result.GetRawText();
                }
            }
            return base.HandleRaw(obj, action, param);
        }
    }
}
