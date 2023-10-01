using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public interface IViewHandler
    {
        IVirtualView VirtualView { get; }
        IPlatformView PlatformView { get; }
        void SetVirtualView(IVirtualView virtualView);
    }
}
