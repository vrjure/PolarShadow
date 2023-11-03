using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls
{
    public interface IWebViewVirtualView : IVirtualView, IWebView
    {

        new IWebViewHandler Handler { get; }
    }
}
