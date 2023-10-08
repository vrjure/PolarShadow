using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.NativeControls;
using Avalonia.Platform;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalonia.NativeControls.Windows
{
    internal class VideoViewHandler : ViewHandler<Avalonia.NativeControls.VideoView, Avalonia.NativeControls.Windows.VideoView>, IVLCHandler
    {
        IVLCPlatformView IVLCHandler.PlatformView => this.PlatformView;

        protected override IPlatformView OnCreatePlatformView()
        {
            return new VideoView(VirtualView);
        }
    }
}
