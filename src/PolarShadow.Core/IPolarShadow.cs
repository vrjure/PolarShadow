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
        bool TryGetSite(string name, out IPolarShadowSite site);
        IEnumerable<IPolarShadowSite> GetSites();
        ISearcHandler BuildSearchHandler(SearchVideoFilter filter);
        IEnumerable<IAnalysisAbility> GetAbilities();
        bool TryGetAbility(string name, out IAnalysisAbility ability);
    }
}
