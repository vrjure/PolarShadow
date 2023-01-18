using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class PolarShadowSiteConfig
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public Dictionary<string, AnalysisAbility> Abilities { get; set; }
    }
}
