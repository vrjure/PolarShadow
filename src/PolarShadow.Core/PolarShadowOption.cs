using PolarShadow.Core.Site;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    public class PolarShadowOption
    {
        public bool IsChanged { get; set; }
        [JsonInclude]
        public ICollection<WebAnalysisSource> AnalysisSources { get; private set; } = new KeyNameCollection<WebAnalysisSource>();
        [JsonInclude]
        public ICollection<SiteOption> Sites { get; private set; } = new KeyNameCollection<SiteOption>();
    }
}
