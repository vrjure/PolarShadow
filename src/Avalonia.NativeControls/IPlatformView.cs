using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public interface IPlatformView : IPlatformHandle
    {
        IPlatformHandle CreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault);
        void DestroyControl(IPlatformHandle handle);
    }
}
