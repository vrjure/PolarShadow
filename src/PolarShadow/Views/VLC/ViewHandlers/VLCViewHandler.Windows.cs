using CommunityToolkit.Maui.Core.Handlers;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using LibVLCSharp.Platforms.Windows;
using LibVLCSharp.Shared;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Media;
using PolarShadow.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Views.ViewHandlers
{
    internal partial class VLCViewHandler : ViewHandler<VLCView, VideoView>
    {
        internal static LibVLC LibVLC;

        protected override VideoView CreatePlatformView()
        {
            return new VideoView();
        }

        protected override void ConnectHandler(VideoView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.Initialized += PlatformView_Initialized;
        }

        protected override void DisconnectHandler(VideoView platformView)
        {
            base.DisconnectHandler(platformView);
            platformView.MediaPlayer?.Stop();
            platformView.Initialized -= PlatformView_Initialized;
            platformView.MediaPlayer?.Dispose();
            platformView.MediaPlayer = null;
        }

        private void PlatformView_Initialized(object sender, InitializedEventArgs e)
        {
            var platformView = sender as VideoView;
            VLCView.InitializeLibVlc(e.SwapChainOptions);
            platformView.MediaPlayer = new MediaPlayer(VLCView.LibVLC);
            VirtualView.MediaPlayer = platformView.MediaPlayer;
        }
    }
}
