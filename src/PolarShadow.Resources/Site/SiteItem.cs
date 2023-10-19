using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal class SiteItem : ISiteItem
    {
        public static readonly string SitesName = "sites";

        internal static IContentWriter _requestWriter = new ContentWriter();
        internal static IContentWriter _responseWriter = new ContentWriter();

        internal readonly IRequestHandler _httpHandler;
        internal readonly IRequestHandler _webViewHandler;


        private readonly ICollection<RequestRule> _requestRules;

        private readonly Dictionary<string, ISite> _sites = new Dictionary<string, ISite>(StringComparer.OrdinalIgnoreCase);
        public SiteItem(IRequestHandler httpHandler, IRequestHandler webViewHandler, IEnumerable<RequestRule> requestRules)
        {
            _httpHandler = httpHandler;
            _webViewHandler = webViewHandler;

            _requestRules = new List<RequestRule>(requestRules);
        }

        public string Name => SitesName;

        public ISite this[string name]
        {
            get => _sites.ContainsKey(name) ? _sites[name] : null;
            set => _sites[name] = value;
        }

        public IEnumerable<ISite> Sites => _sites.Values;

        public IRequestHandler HttpHandler => _httpHandler;

        public IRequestHandler WebViewHandler => _webViewHandler;

        public void Load(IPolarShadowProvider provider, bool reLoad = false)
        {
            if (reLoad)
            {
                _sites.Clear();
            }

            if (!provider.Root.TryGetProperty(SitesName, out JsonElement sitesValue))
            {
                return;
            }
            BuildSites(sitesValue);
        }

        private void BuildSites(JsonElement sitesValue)
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

        private ISite BuildSite(JsonElement siteConfig)
        {
            var site = JsonSerializer.Deserialize<SiteDefault>(siteConfig, JsonOption.DefaultSerializer);
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
                JsonSerializer.Serialize(writer, site, JsonOption.DefaultSerializer);
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
