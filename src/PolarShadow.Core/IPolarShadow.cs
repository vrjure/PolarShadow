using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPolarShadow
    {
        IPolarShadowBuilder Builder { get; }
        NameSlotValueCollection Parameters { get; }
        bool ContainsSite(string name);
        bool TryGetSite(string name, out IPolarShadowSite site);
        IEnumerable<IPolarShadowSite> GetSites();
    }
}
