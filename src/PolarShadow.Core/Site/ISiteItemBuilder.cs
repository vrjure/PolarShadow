using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface ISiteItemBuilder : IPolarShadowItemBuilder
    {
        ISiteBuilder SiteBuilder { get; set; }
        IParameter Parameter { get; }
    }
}
