using PolarShadow.Core.Site;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class PolarShadowOption
    {
        public bool IsChanged { get; set; }
        public ICollection<HtmlAnalysisSource> AnalysisSources { get; set; } = new List<HtmlAnalysisSource>();
        public ICollection<SiteOption> Sites { get; set; } = new List<SiteOption>();
    }
}
