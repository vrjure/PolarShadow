using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class JsonAnalysisHandler : AnalysisActionHandler<JsonElement>
    {
        private readonly JsonElement _input;
        public JsonAnalysisHandler(JsonElement input)
        {
            _input = input;
        }
        protected override bool VerifyInput(JsonElement obj)
        {
            return obj.ValueKind != JsonValueKind.Undefined;
        }

        protected override IEnumerable<JsonElement> HandleArray(JsonElement obj, AnalysisAction action)
        {
            var path = action.Path.NameSlot(_input);
            if (obj.TryGetPropertyWithJsonPath(path, out JsonElement element))
            {
                return element.EnumerateArray();
            }
            return null;
        }

        protected override string HandleString(JsonElement obj, AnalysisAction action)
        {
            var path = action.Path.NameSlot(_input);
            if (obj.TryGetPropertyWithJsonPath(path, out JsonElement element))
            {
                return element.GetString();
            }
            return string.Empty;
        }

        protected override bool? HandleBoolean(JsonElement obj, AnalysisAction action)
        {
            var path = action.Path.NameSlot(_input);
            if (obj.TryGetPropertyWithJsonPath(path, out JsonElement element))
            {
                return element.GetBoolean();
            }
            return default;
        }

        protected override decimal? HandleNumber(JsonElement obj, AnalysisAction action)
        {
            var path = action.Path.NameSlot(_input);
            if (obj.TryGetPropertyWithJsonPath(path, out JsonElement element) && element.TryGetDecimal(out decimal value))
            {
                return value;
            }

            return default;
        }

        protected override JsonElement HandleNext(JsonElement obj, AnalysisAction action)
        {
            var path = action.Path.NameSlot(_input);
            if (obj.TryGetPropertyWithJsonPath(path, out JsonElement element))
            {
                return element;
            }
            return default;
        }

        protected override string HandleRaw(JsonElement obj, AnalysisAction action)
        {
            var path = action.Path.NameSlot(_input);
            if (obj.TryGetPropertyWithJsonPath(path, out JsonElement element))
            {
                return element.GetRawText();
            }
            return default;
        }
    }
}
