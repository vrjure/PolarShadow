using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public class AnalysisRequest
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public JsonElement? Body { get; set; }
    }
}
