using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls.Android
{
    internal class VideoViewHandler : ViewHandler<Avalonia.NativeControls.VideoView, Avalonia.NativeControls.Android.VideoView>, IVLCHandler
    {
        IVLCPlatformView IVLCHandler.PlatformView => this.PlatformView;

        protected override IPlatformView OnCreatePlatformView()
        {
            return new VideoView(VirtualView);
        }

        protected override void ConnectHandler(VideoView platformView)
        {
            platformView.PlatformClick += PlatformView_PlatformClick;
        }

        protected override void DisconnectHandler(VideoView platformView)
        {
            platformView.PlatformClick -= PlatformView_PlatformClick;
        }

        private void PlatformView_PlatformClick(object sender, EventArgs e)
        {
            this.VirtualView?.OnPlatformClick();
        }
    }
}
