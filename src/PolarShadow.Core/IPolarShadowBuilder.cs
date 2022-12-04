using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPolarShadowBuilder
    {
        void AddSite(IPolarShadowSite site);
        void AddSearcHandlerFactory(Func<SearchVideoFilter, ISearcHandler> factory);
        IPolarShadow Build();
    }
}
