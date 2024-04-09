using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PolarShadow.WebView
{
    public interface IWebViewRequestHandler : IRequestHandler
    {
        void SetContainer(Panel container);
    }
}
