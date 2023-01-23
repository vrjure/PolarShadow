using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal sealed class PolarShadowSiteBuilder : IPolarShadowSiteBuilder
    {
        private readonly PolarShadowSiteConfig _config;
        private readonly PolarShadowBuilder _builder;
        public PolarShadowSiteBuilder(PolarShadowSiteConfig config, PolarShadowBuilder builder)
        {
            if (string.IsNullOrEmpty(config.Name))
            {
                throw new ArgumentException(nameof(config.Name));
            }
            _config = config;
            _builder = builder;
        }

        public IPolarShadowSite Build()
        {
            var abilities = new List<KeyValuePair<string, object>>(_config.Abilities.Count);
            
            foreach (var item in _config.Abilities)
            {
                if (_builder._supportAbilityFactories.TryGetValue(item.Key, out IAbilityFactory factory))
                {
                    abilities.Add(new KeyValuePair<string, object>(item.Key, factory.Create(item.Value)));
                }
            }

            return new PolarShadowSiteDefault(_config.Name, _config.Domain, abilities);
        }
    }
}
