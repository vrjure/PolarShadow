using CommunityToolkit.Maui.Core.Handlers;
using CommunityToolkit.Maui.Views;
using LibVLCSharp.Platforms.Android;
using Microsoft.Maui.Handlers;
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
        protected override VideoView CreatePlatformView()
        {
            return new VideoView(Context);
        }

        protected override void ConnectHandler(VideoView platformView)
        {
            base.ConnectHandler(platformView);
        }
    }
}
