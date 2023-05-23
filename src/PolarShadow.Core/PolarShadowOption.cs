using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    public class PolarShadowOption
    {
        public bool IsChanged { get; set; }
        public JsonElement Parameters { get; set; }
        [JsonInclude]
        public ICollection<WebAnalysisSource> AnalysisSources { get; private set; } = new KeyNameCollection<WebAnalysisSource>();
        [JsonInclude]
        public ICollection<PolarShadowSiteOption> Sites { get; private set; } = new KeyNameCollection<PolarShadowSiteOption>();
    }
}
