using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Handlers;

namespace PolarShadow.Controls
{
    partial class VideoHandler: ViewHandler<VideoPlayer, PolarShadow.Platforms.Android.VideoPlayer>
    {
        protected override Platforms.Android.VideoPlayer CreatePlatformView()
        {
            return new Platforms.Android.VideoPlayer(Context, VirtualView);
        }

        protected override void ConnectHandler(Platforms.Android.VideoPlayer platformView)
        {
            base.ConnectHandler(platformView);
            // Perform any control setup here
        }

        protected override void DisconnectHandler(PolarShadow.Platforms.Android.VideoPlayer platformView)
        {
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }

        public static void MapAreTransportControlsEnabled(VideoHandler handler, VideoPlayer video)
        {
            handler.PlatformView?.UpdateTransportControlsEnabled();
        }

        public static void MapSource(VideoHandler handler, VideoPlayer video)
        {
            handler.PlatformView?.UpdateSource();
        }

        public static void MapIsLooping(VideoHandler handler, VideoPlayer video)
        {
            handler.PlatformView?.UpdateIsLooping();
        }

        public static void MapPosition(VideoHandler handler, VideoPlayer video)
        {
            handler.PlatformView?.UpdatePosition();
        }

        public static void MapUpdateStatus(VideoHandler handler, VideoPlayer video, object? args)
        {
            handler.PlatformView?.UpdateStatus();
        }

        public static void MapPlayRequested(VideoHandler handler, VideoPlayer video, object? args)
        {
            if (args is not VideoPositionEventArgs)
                return;

            TimeSpan position = ((VideoPositionEventArgs)args).Position;
            handler.PlatformView?.PlayRequested(position);
        }

        public static void MapPauseRequested(VideoHandler handler, VideoPlayer video, object? args)
        {
            if (args is not VideoPositionEventArgs)
                return;

            TimeSpan position = ((VideoPositionEventArgs)args).Position;
            handler.PlatformView?.PauseRequested(position);
        }

        public static void MapStopRequested(VideoHandler handler, VideoPlayer video, object? args)
        {
            if (args is not VideoPositionEventArgs)
                return;

            TimeSpan position = ((VideoPositionEventArgs)args).Position;
            handler.PlatformView?.StopRequested(position);
        }
    }
}
