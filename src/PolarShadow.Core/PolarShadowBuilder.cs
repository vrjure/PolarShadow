using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class PolarShadowBuilder : IPolarShadowBuilder
    {
        private static object _lock = new object();
        private Func<SearchVideoFilter, ISearcHandler> _searcHandlerFactory;
        private IPolarShadow _cache;

        public PolarShadowOption Option { get; }
        public IPolarShadowSiteBuilder SiteBuilder { get; set; }

        public PolarShadowBuilder() : this(default)
        {
        }

        public PolarShadowBuilder(PolarShadowOption option)
        {
            this.Option = option;
            if (this.Option == null)
            {
                this.Option = new PolarShadowOption();
            }
        }

        public void AddSearcHandlerFactory(Func<SearchVideoFilter, ISearcHandler> factory)
        {
            _searcHandlerFactory = factory;
        }

        public IPolarShadow Build()
        {
            lock(_lock )
            {
                if (SiteBuilder == null)
                {
                    SiteBuilder = new PolarShadowSiteBuilder();
                }

                if (Option.IsChanged || _cache == null)
                {
                    //var ps = new PolarShadowDefault(Option, SiteBuilder, _searcHandlerFactory);
                    Option.IsChanged = false;
                    //_cache = ps;
                }

                return _cache;
            }
        }
    }
}
