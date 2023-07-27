using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface ISiteItemBuilder : IPolarShadowItemBuilder
    {
        IRequestHandler WebViewHandler { get; set; }
        IRequestHandler HttpHandler { get; set; }
        ICollection<IContentBuilder> RequestBuilders { get; }
        ICollection<IContentBuilder> ResponseBuilders { get; }
    }
}
