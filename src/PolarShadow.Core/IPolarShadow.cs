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
        IPolarShadowSite GetSite(string name);
        IEnumerable<IPolarShadowSite> GetSites();
        ISearcHandler BuildSearchHandler(SearchVideoFilter filter);
    }
}
