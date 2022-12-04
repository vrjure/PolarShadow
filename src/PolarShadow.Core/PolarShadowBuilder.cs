using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class PolarShadowBuilder : IPolarShadowBuilder
    {
        private readonly List<IPolarShadowSite> _sites = new List<IPolarShadowSite>();
        private Func<SearchVideoFilter, ISearcHandler> _searcHandlerFactory;

        public void AddSearcHandlerFactory(Func<SearchVideoFilter, ISearcHandler> factory)
        {
            _searcHandlerFactory = factory;
        }

        public void AddSite(IPolarShadowSite site)
        {
            _sites.Add(site);
        }


        public IPolarShadow Build()
        {
            return new PolarShadowDefault(_sites, _searcHandlerFactory);
        }
    }
}
