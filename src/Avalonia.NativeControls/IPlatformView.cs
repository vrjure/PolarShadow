using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public interface IPlatformView
    {
        IPlatformHandle CreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault);
        void DestroyControl(IPlatformHandle handle);
    }
}
