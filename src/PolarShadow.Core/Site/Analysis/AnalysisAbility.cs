using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class AnalysisAbility
    {
        public string Url { get; set; }
        public AnalysisType AnalysisType { get; set; }
        public string Encoding { get; set; }
        public Dictionary<string, AnalysisAction> Analysis { get; set; }
        public AnalysisAbility Next { get; set; }
    }
}
