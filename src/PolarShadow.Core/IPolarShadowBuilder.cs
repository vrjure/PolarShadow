﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPolarShadowBuilder
    {
        IPolarShadowBuilder UseWebViewHandler(IRequestHandler requestHandler);
        IPolarShadowBuilder UseSiteBuilder(IPolarShadowSiteBuilder siteBuilder);
        IPolarShadowBuilder UseOptionBuilder(IPolarShadowOptionBuilder optionBuilder);
        IPolarShadowBuilder Configure(Action<IPolarShadowOptionBuilder> optionBuilder);
        IPolarShadow Build();
    }
}
