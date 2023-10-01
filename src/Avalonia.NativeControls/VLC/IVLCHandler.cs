using Avalonia.NativeControls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public interface IVLCHandler : IViewHandler
    {
        new IVLCPlatformView PlatformView { get; }
    }
}
