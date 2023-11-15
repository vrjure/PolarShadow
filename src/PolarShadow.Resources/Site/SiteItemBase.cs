using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    public abstract class SiteItemBase<TSite> : ISiteItem<TSite> where TSite : ISite
    {
        private readonly IRequestHandler _httpHandler;
        private readonly IRequestHandler _webViewHandler;
        private readonly ICollection<RequestRule> _requestRules;
        private readonly Dictionary<string, TSite> _sites = new Dictionary<string, TSite>(StringComparer.OrdinalIgnoreCase);

        public SiteItemBase(IRequestHandler httpHandler, IRequestHandler webViewHandler, IEnumerable<RequestRule> requestRules)
        {
            _httpHandler = httpHandler;
            _webViewHandler = webViewHandler;
            _requestRules = new List<RequestRule>(requestRules);
        }

        public abstract string Name { get; }


        public IEnumerable<TSite> Sites => _sites.Values;
        IEnumerable<ISite> ISiteItem.Sites => _sites.Values.Cast<ISite>();

        public IRequestHandler HttpHandler => _httpHandler;
        public IRequestHandler WebViewHandler => _webViewHandler;

        public TSite this[string name]
        {
            get => _sites.ContainsKey(name) ? _sites[name] : default;
            set => _sites[name] = value;
        }
        ISite ISiteItem.this[string name]
        {
            get => _sites.ContainsKey(name) ? _sites[name] : default;
            set => _sites[name] = (TSite)value;
        }

        public void Load(IPolarShadowProvider provider, bool reLoad = false)
        {
            if (reLoad)
            {
                _sites.Clear();
            }

            if (!provider.Root.TryGetProperty(Name, out JsonElement sitesValue))
            {
                return;
            }
            BuildSites(sitesValue);
        }


        protected virtual void BuildSites(JsonElement sitesValue)
        {
            if (sitesValue.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            foreach (var item in sitesValue.EnumerateArray())
            {
                var site = BuildSite(item);
                _sites[site.Name] = site;
            }
        }

        protected virtual TSite BuildSite(JsonElement siteConfig)
        {
            var site = JsonSerializer.Deserialize<TSite>(siteConfig, JsonOption.DefaultSerializer);
            return site;
        }

        public void Remove(string name)
        {
            _sites.Remove(name);
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            writer.WriteStartArray();
            foreach (var site in Sites)
            {
                JsonSerializer.Serialize(writer, (object)site, JsonOption.DefaultSerializer);
            }
            writer.WriteEndArray();
        }

        public void LoadFrom(IPolarShadowSource source)
        {
            var provider = source.Build(null);
            provider.Load();
            _sites.Clear();
            if (provider.Root.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            BuildSites(provider.Root);
        }

        public IEnumerable<RequestRule> EnumeratorRequestRules(string requestName = "")
        {
            if (_requestRules?.Count > 0)
            {
                if (string.IsNullOrEmpty(requestName))
                {
                    foreach (var item in _requestRules)
                    {
                        yield return item;
                    }
                }
                else
                {
                    foreach (var item in _requestRules)
                    {
                        if (item.RequestName == "*" || item.RequestName.Equals(requestName) || RequestRule.MatchWithWildcard(requestName, item.RequestName))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }
    }
}
