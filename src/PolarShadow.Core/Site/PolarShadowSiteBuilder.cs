using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal sealed class PolarShadowSiteBuilder : IPolarShadowSiteBuilder
    {
        private readonly Dictionary<string, object> _support;
        private PolarShadowSiteConfig _config;
        public PolarShadowSiteBuilder(PolarShadowSiteConfig config, Dictionary<string, object> abilities)
        {
            if (string.IsNullOrEmpty(config.Name))
            {
                throw new ArgumentException(nameof(config.Name));
            }
            _config = config;
        }

        public IPolarShadowSite Build()
        {
            throw new NotImplementedException();
        }
    }
}
