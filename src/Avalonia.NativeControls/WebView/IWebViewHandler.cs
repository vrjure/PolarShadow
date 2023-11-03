using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls
{
    public interface IWebViewHandler : IViewHandler
    {
        new IWebViewPlatformView PlatformView { get; }
    }
}
