using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class HtmlAnalysisHandler : AnalysisActionHandler<HtmlNode>
    {
        private readonly HtmlNode _input;
        private readonly JsonElement _param;
        public HtmlAnalysisHandler(HtmlNode input, JsonElement param)
        {
            _input = input;
            _param = param;
        }
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

        protected override HtmlNode HandleNext(HtmlNode obj, AnalysisAction action)
        {
            return obj.SelectSingleNode(action.Path);
        }

        protected override IEnumerable<HtmlNode> HandleArray(HtmlNode obj, AnalysisAction action)
        {
            return obj.SelectNodes(action.Path);
        }

        protected override string HandleString(HtmlNode obj, AnalysisAction action)
        {
            if (_param.ValueKind != JsonValueKind.Undefined)
            {
                var path = action.Path.NameSlot(_param);
                if (_param.TryGetPropertyWithJsonPath(path, out JsonElement result))
                {
                    return result.GetString();
                }
            }

            return base.HandleString(obj, action);

        }

        protected override bool? HandleBoolean(HtmlNode obj, AnalysisAction action)
        {
            if (_param.ValueKind != JsonValueKind.Undefined)
            {
                var path = action.Path.NameSlot(_param);
                if (_param.TryGetPropertyWithJsonPath(path, out JsonElement result))
                {
                    return result.GetBoolean();
                }
            }
            return base.HandleBoolean(obj, action);
        }

        protected override decimal? HandleNumber(HtmlNode obj, AnalysisAction action)
        {
            if (_param.ValueKind != JsonValueKind.Undefined)
            {
                var path = action.Path.NameSlot(_param);
                if (_param.TryGetPropertyWithJsonPath(path, out JsonElement result))
                {
                    return result.GetDecimal();
                }
            }
            return base.HandleNumber(obj, action);
        }

        protected override string HandleRaw(HtmlNode obj, AnalysisAction action)
        {
            if (_param.ValueKind != JsonValueKind.Undefined)
            {
                var path = action.Path.NameSlot(_param);
                if (_param.TryGetPropertyWithJsonPath(path, out JsonElement result))
                {
                    return result.GetRawText();
                }
            }
            return base.HandleRaw(obj, action);
        }
    }
}
