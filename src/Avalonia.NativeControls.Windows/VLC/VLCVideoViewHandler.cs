using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalonia.Controls.Windows
{
    internal class VLCVideoViewHandler : ViewHandler<Avalonia.Controls.VideoView, Avalonia.Controls.Windows.VLCVideoView>, IVideoViewHandler
    {
        IPlatformVideoView IVideoViewHandler.PlatformView => this.PlatformView;

        protected override IPlatformView OnCreatePlatformView()
        {
            return new VLCVideoView(VirtualView);
        }

        protected override void ConnectHandler(VLCVideoView platformView)
        {
            VirtualView.Controller = new VLController();
        }

        protected override void DisconnectHandler(VLCVideoView platformView)
        {
            VirtualView.Controller?.Dispose();
        }
    }
}
