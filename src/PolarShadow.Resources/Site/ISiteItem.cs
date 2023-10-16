using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public interface ISiteItem : IPolarShadowItem
    {
        IRequestHandler HttpHandler { get; }
        IRequestHandler WebViewHandler { get; }
        ISite this[string name] { get; set; }
        IEnumerable<ISite> Sites { get; }
        void Remove(string name);
        IEnumerable<RequestRule> EnumeratorRequestRules(string requestName = "");
    }
}
