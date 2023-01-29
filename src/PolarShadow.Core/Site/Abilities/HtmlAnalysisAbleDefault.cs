using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class HtmlAnalysisAbleDefault : AnalysisAbilityHandler, IHtmlAnalysisAble
    {
        public HtmlAnalysisAbleDefault(AnalysisAbility ability) : base(ability)
        {
        }

        public string GetAnalysisedSource(VideoSource source, HtmlAnalysisSource analysisSource)
        {
            var doc = JsonDocument.Parse(JsonSerializer.Serialize(source, JsonOption.DefaultSerializer));
            return analysisSource.Src.NameSlot(doc.RootElement);
        }
    }
}
