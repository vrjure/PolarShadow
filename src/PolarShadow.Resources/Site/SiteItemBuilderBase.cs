using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public abstract class SiteItemBuilderBase : ISiteItemBuilder
    {
        public IRequestHandler WebViewHandler { get; set; }
        public IRequestHandler HttpHandler { get; set; }
        public ICollection<RequestRule> RequestRules { get; } = new List<RequestRule>();

        public abstract IPolarShadowItem Build(string name, IPolarShadowBuilder builder);
    }
}
