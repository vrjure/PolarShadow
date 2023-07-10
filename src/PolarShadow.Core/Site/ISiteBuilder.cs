using PolarShadow.Core.Site;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface ISiteBuilder
    {
        ISite Build(IPolarShadowBuilder builder, ISiteItemBuilder itemBuilder, JsonElement siteConfig);
    }
}
