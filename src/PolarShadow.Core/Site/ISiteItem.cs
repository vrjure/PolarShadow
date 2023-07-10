using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface ISiteItem : IPolarShadowItem
    {
        ISite this[string name] { get; }
        IEnumerable<ISite> Sites { get; }
    }
}
