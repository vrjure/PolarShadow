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
        IEnumerable<IPolarShadowItem> Items { get; }
        IPolarShadowItem this[string name] { get; }
    }
}
