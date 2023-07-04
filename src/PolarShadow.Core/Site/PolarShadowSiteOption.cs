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
        public bool Enable { get; set; } = true;
        public Dictionary<string, object> Parameters { get; set; }
        public Dictionary<string, AnalysisAbility> Abilities { get; set; }

        public void Apply(PolarShadowSiteOption other)
        {
            this.Parameters = other.Parameters;
            this.Domain = other.Domain;
            this.Enable = other.Enable;
            this.Abilities = other.Abilities;
            this.UseWebView = other.UseWebView;
        }
    }
}
