using Avalonia.Controls;
using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Handlers
{
    public interface IWebViewRequestHandler : IRequestHandler
    {
        void SetContainer(Panel container);
    }
}
