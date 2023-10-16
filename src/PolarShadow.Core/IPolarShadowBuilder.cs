using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPolarShadowBuilder
    {
        IPolarShadowBuilder Add(IPolarShadowItemBuilder builder);
        IEnumerable<IPolarShadowItemBuilder> ItemBuilders { get; }
        IPolarShadow Build();
    }
}
