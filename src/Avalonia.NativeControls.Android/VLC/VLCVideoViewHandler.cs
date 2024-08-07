﻿using CommunityToolkit.Mvvm.DependencyInjection;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls.Android
{
    internal class VLCVideoViewHandler : ViewHandler<Avalonia.Controls.VideoView, Avalonia.Controls.Android.VLCVideoView>, IVideoViewHandler
    {
        IPlatformVideoView IVideoViewHandler.PlatformView => this.PlatformView;

        protected override IPlatformView OnCreatePlatformView()
        {
            return new VLCVideoView(VirtualView);
        }

        protected override void ConnectHandler(VLCVideoView platformView)
        {
            //VirtualView.Controller = Ioc.Default.GetRequiredService<IVideoViewController>();
        }

        protected override void DisconnectHandler(VLCVideoView platformView)
        {
            VirtualView.Controller?.Dispose();
        }
    }
}
