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
        private readonly NameSlotValueCollection _parameter;

        internal PolarShadowDefault(IPolarShadowBuilder builder, IEnumerable<IPolarShadowSite> sites, NameSlotValueCollection parameter)
        {
            _builder = builder;
            _parameter = parameter;
            foreach (var item in sites)
            {
                _sites.Add(item.Name, item);
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public IPolarShadowBuilder Builder => _builder;

        public NameSlotValueCollection Parameters => _parameter.Clone();

        public bool ContainsSite(string name)
        {
            return _sites.ContainsKey(name);
        }

        public IEnumerable<IPolarShadowSite> GetSites()
        {
            return new List<IPolarShadowSite>(_sites.Values);
        }

        public bool TryGetSite(string name, out IPolarShadowSite site)
        {
            return _sites.TryGetValue(name, out site);
        }
    }
}
