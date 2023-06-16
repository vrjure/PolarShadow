﻿using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    public class PolarShadowOption
    {
        public Dictionary<string, object> Parameters { get; set; }
        [JsonInclude]
        public ICollection<WebAnalysisSource> AnalysisSources { get; set; } = new KeyNameCollection<WebAnalysisSource>();
        [JsonInclude]
        public ICollection<PolarShadowSiteOption> Sites { get; private set; } = new KeyNameCollection<PolarShadowSiteOption>();

        public void Apply(PolarShadowOption other)
        {
            Parameters = other.Parameters == null ? new Dictionary<string, object>() : new Dictionary<string, object>(other.Parameters);
            AnalysisSources = other.AnalysisSources == null ? new KeyNameCollection<WebAnalysisSource>() : new KeyNameCollection<WebAnalysisSource>(other.AnalysisSources);
            Sites = other.Sites == null ? new KeyNameCollection<PolarShadowSiteOption>() : new KeyNameCollection<PolarShadowSiteOption>(other.Sites);
        }
    }
}
