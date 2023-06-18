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
        bool IsOptionChanged { get; }
        IPolarShadowBuilder UseWebViewHandler(IRequestHandler requestHandler);
        IPolarShadowBuilder Configure(Action<IPolarShadowOptionBuilder> optionBuilder);
        IPolarShadow Build();
    }
}
