using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Handlers;

#if ANDROID
using PlatformView = PolarShadow.Platforms.Android.VideoPlayer;
#elif WINDOWS
using PlatformView = PolarShadow.Platforms.Windows.VideoPlayer;
#endif

namespace PolarShadow.Controls
{
    partial class VideoHandler
    {
        public static IPropertyMapper<VideoPlayer, VideoHandler> PropertyMapper = new PropertyMapper<VideoPlayer, VideoHandler>(ViewHandler.ViewMapper)
        {
            [nameof(VideoPlayer.AreTransportControlsEnabled)] = MapAreTransportControlsEnabled,
            [nameof(VideoPlayer.Source)] = MapSource,
            [nameof(VideoPlayer.IsLooping)] = MapIsLooping,
            [nameof(VideoPlayer.Position)] = MapPosition
        };

        public static CommandMapper<VideoPlayer, VideoHandler> CommandMapper = new(ViewCommandMapper)
        {
            [nameof(VideoPlayer.UpdateStatus)] = MapUpdateStatus,
            [nameof(VideoPlayer.PlayRequested)] = MapPlayRequested,
            [nameof(VideoPlayer.PauseRequested)] = MapPauseRequested,
            [nameof(VideoPlayer.StopRequested)] = MapStopRequested
        };
        

        public VideoHandler():base(PropertyMapper, CommandMapper)
        {

        }
    }
}
