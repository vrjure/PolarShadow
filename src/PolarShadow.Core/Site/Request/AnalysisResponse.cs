using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public class AnalysisResponse
    {
        public string Encoding { get; set; }
        public JsonElement? Template { get; set; }
    }
}
