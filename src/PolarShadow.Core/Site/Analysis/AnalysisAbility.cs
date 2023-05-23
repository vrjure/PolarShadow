using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public class AnalysisAbility
    {
        public AnalysisRequest Request { get; set; }
        public AnalysisResponse Response { get; set; }
        public JsonElement? Parameters { get; set; }
        public AnalysisAbility Next { get; set; }
    }
}
