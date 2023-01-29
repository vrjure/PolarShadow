using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPolarShadowBuilder
    {
        PolarShadowOption Option { get; }
        void AddSearcHandlerFactory(Func<SearchVideoFilter, ISearcHandler> factory);
        void RegisterSupportAbilityFactory<T>(string name, IAbilityFactory<T> ability);
        IPolarShadow Build();
    }
}
