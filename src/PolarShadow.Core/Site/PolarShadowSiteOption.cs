using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    internal class PolarShadowSiteOption : IKeyName
    {
        public PolarShadowSiteOption() { }
        public PolarShadowSiteOption(string name) 
        {
            this.Name = name;
        }

        [JsonRequired]
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool UseWebView { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public Dictionary<string, AnalysisAbility> Abilities { get; set; }
    }
}
