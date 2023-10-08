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
    }
}
