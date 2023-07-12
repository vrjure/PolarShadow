using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class SiteItem : ISiteItem
    {
        public static readonly string SitesName = "sites";
        private readonly Dictionary<string, ISite> _sites = new Dictionary<string, ISite>();
        public SiteItem(IEnumerable<ISite> sites)
        {
            foreach (var site in sites)
            {
                _sites[site.Name] = site;
            }
        }
        public string Name => SitesName;

        public ISite this[string name] => _sites.ContainsKey(name) ? _sites[name] : null;

        public IEnumerable<ISite> Sites => _sites.Values;

        public void Write(Utf8JsonWriter writer)
        {
            writer.WriteStartArray(Name);
            foreach (var site in Sites)
            {

            }
            writer.WriteEndArray();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
    }
}
