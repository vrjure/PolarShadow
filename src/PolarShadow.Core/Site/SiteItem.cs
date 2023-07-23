using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class SiteItem : ISiteItem
    {
        public static readonly string SitesName = "sites";
        private readonly IRequestHandler _httpHandler;
        private readonly IRequestHandler _webViewHandler;
        private readonly IParameter _globalParameter;
        private readonly Dictionary<string, ISite> _sites = new Dictionary<string, ISite>(StringComparer.OrdinalIgnoreCase);
        public SiteItem(IRequestHandler httpHandler, IRequestHandler webViewHandler, IParameter globalParameters)
        {
            _httpHandler = httpHandler;
            _webViewHandler = webViewHandler;
            _globalParameter = globalParameters;
        }

        public string Name => SitesName;

        public ISite this[string name]
        {
            get => _sites.ContainsKey(name) ? _sites[name] : null;
            set => _sites[name] = value;
        }

        public IEnumerable<ISite> Sites => _sites.Values;

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
            var p = new Parameters();
            if (_globalParameter != null)
            {
                p.Add(_globalParameter);
            }

            if (site.Parameters != null)
            {
                p.Add(site.Parameters);
            }
            site.ParametersInternal = p;

            site.HttpRequestHandlerInternal = _httpHandler;
            site.WebViewRequestHandlerInteral = _webViewHandler;

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
                site.WriteTo(writer);
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
    }
}
