using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public class PolarShadowSiteOption : IKeyName
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public JsonElement? Parameters { get; set; }
        public Dictionary<string, AnalysisAbility> Abilities { get; set; }
    }
}
