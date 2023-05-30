using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class PolarShadowDefault : IPolarShadow
    {
        private readonly Dictionary<string, IPolarShadowSite> _sites = new Dictionary<string, IPolarShadowSite>();
        private readonly IPolarShadowBuilder _builder;
        internal PolarShadowDefault(IPolarShadowBuilder builder, IEnumerable<IPolarShadowSite> sites)
        {
            _builder = builder;
            foreach (var item in sites)
            {
                _sites.Add(item.Name, item);
            }
        }

        public IPolarShadowBuilder Builder => _builder;

        public bool ContainsSite(string name)
        {
            return _sites.ContainsKey(name);
        }

        public IEnumerable<IPolarShadowSite> GetSites()
        {
            return _sites.Values.AsEnumerable();
        }

        public bool TryGetSite(string name, out IPolarShadowSite site)
        {
            return _sites.TryGetValue(name, out site);
        }
    }
}
