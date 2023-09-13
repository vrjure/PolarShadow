using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public interface ISiteItemBuilder : IPolarShadowItemBuilder
    {
        IRequestHandler WebViewHandler { get; set; }
        IRequestHandler HttpHandler { get; set; }
        ICollection<IContentWriting> Writings { get; }
    }
}
