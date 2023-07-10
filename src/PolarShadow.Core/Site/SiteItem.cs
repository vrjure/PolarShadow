using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Site
{
    internal class SiteItem : ISiteItem
    {
        private readonly Dictionary<string, ISite> _sites = new Dictionary<string, ISite>();
        public SiteItem(IEnumerable<ISite> sites)
        {
            foreach (var site in sites)
            {
                _sites[site.Name] = site;
            }
        }
        public string Name => "sites";

        public ISite this[string name] => _sites.ContainsKey(name) ? _sites[name] : null;

        public IEnumerable<ISite> Sites => _sites.Values;
    }
}
