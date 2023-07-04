using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class AnalysisAbility
    {
        public AnalysisRequest Request { get; set; }
        public AnalysisResponse Response { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public AnalysisAbility Next { get; set; }
    }
}
