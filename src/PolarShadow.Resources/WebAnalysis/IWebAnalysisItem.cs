using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public interface IWebAnalysisItem : IPolarShadowItem
    {
        WebAnalysisSource this[string name] { get; set; }
        IEnumerable<WebAnalysisSource> Sources { get; }
        void Remove(string name);
    }
}
