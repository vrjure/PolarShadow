using Avalonia.NativeControls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public interface IVLCVirtualView : IVirtualView, IVideoView
    {
        new IVLCHandler Handler { get; }
        void OnPlatformClick();
    }
}
