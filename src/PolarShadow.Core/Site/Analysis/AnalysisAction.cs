using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class AnalysisAction
    {
        public string Path { get; set; }
        public PathValueType PathValueType { get; set; }
        public string AttributeName { get; set; }
        public string Regex { get; set; }
        public AnalysisAction Next { get; set; }
        public Dictionary<string, AnalysisAction> AnalysisItem { get; set; }
    }
}
