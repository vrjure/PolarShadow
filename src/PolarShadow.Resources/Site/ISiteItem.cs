using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public interface ISiteItem : IPolarShadowItem
    {
        ISite this[string name] { get; set; }
        IEnumerable<ISite> Sites { get; }
        void Remove(string name);
    }
}
