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
        IRequestHandler WebViewHandler { get; set; }
        IRequestHandler HttpHandler { get; set; }
        IParameterCollection Parameters { get; }
        IPolarShadowBuilder Add(IPolarShadowItemBuilder builder);
        IPolarShadowBuilder Add(IPolarShadowSource source);
        IEnumerable<IPolarShadowItemBuilder> ItemBuilders { get; }
        IEnumerable<IPolarShadowSource> Sources { get; }
        IPolarShadow Build();
    }
}
