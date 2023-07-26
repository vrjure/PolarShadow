using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface ISiteItemBuilder : IPolarShadowItemBuilder
    {
        IRequestHandler WebViewHandler { get; set; }
        IRequestHandler HttpHandler { get; set; }
        IDictionary<string, IContentBuilder> RequestBuilders { get; set; }
        IDictionary<string, IContentBuilder> ResponseBuilders { get; set; }
    }
}
