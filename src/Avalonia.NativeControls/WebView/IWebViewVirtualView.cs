using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public interface IWebViewVirtualView : IVirtualView, IWebView
    {

        new IWebViewHandler Handler { get; }
    }
}
