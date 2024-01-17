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
            platformView.PlatformClick += PlatformView_PlatformClick;
            platformView.Controller = new VLController();
        }

        protected override void DisconnectHandler(VLCVideoView platformView)
        {
            platformView.PlatformClick -= PlatformView_PlatformClick;
            platformView.Controller?.Dispose();
        }

        private void PlatformView_PlatformClick(object sender, EventArgs e)
        {
            this.VirtualView?.OnPlatformClick();
        }
    }
}
