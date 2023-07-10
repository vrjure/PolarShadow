using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Core.Configurations
{
    internal class NoLoadConfigurationRoot : IConfigurationRoot
    {
        private readonly IList<IConfigurationProvider> _providers;

        public NoLoadConfigurationRoot(IList<IConfigurationProvider> providers)
        {
            _providers = providers;
        }

        public string this[string key]
        {
            get => GetConfiguration(_providers, key);
            set => SetConfiguration(_providers, key, value);
        }

        public IEnumerable<IConfigurationProvider> Providers => _providers;

        public IEnumerable<IConfigurationSection> GetChildren() => GetChildrenInternal();

        IChangeToken IConfiguration.GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
            => new ConfigurationSection(this, key);

        void IConfigurationRoot.Reload()
        {
            throw new NotImplementedException();
        }

        internal static string GetConfiguration(IList<IConfigurationProvider> providers, string key)
        {
            for (int i = providers.Count - 1; i >= 0; i--)
            {
                IConfigurationProvider provider = providers[i];

                if (provider.TryGet(key, out string value))
                {
                    return value;
                }
            }

            return null;
        }

        internal static void SetConfiguration(IList<IConfigurationProvider> providers, string key, string? value)
        {
            if (providers.Count == 0)
            {
                throw new InvalidOperationException("Source is empty");
            }

            foreach (IConfigurationProvider provider in providers)
            {
                provider.Set(key, value);
            }
        }

        internal IEnumerable<IConfigurationSection> GetChildrenInternal()
        {
            IEnumerable<IConfigurationSection> children = _providers
                .Aggregate(Enumerable.Empty<string>(),
                    (seed, source) => source.GetChildKeys(seed, default))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(key => this.GetSection(key));

                return children.ToList();
        }
    }
}
