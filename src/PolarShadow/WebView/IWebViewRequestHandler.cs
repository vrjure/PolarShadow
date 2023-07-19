using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    public interface IWebViewRequestHandler : IRequestHandler
    {
        void SetContainer(IContainer container);
    }
}
