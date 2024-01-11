using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Controls
{
    public interface IViewHandler
    {
        IVirtualView VirtualView { get; }
        IPlatformView PlatformView { get; }
        void SetVirtualView(IVirtualView virtualView);
        void DisconnectHandler();
    }
}
