using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public interface IMediaPlayerController
    {
        IVideoViewController Controller { get; set; }
        string Title { get; set; }
        TimeSpan Length { get; }
        TimeSpan Time { get; }
        bool IsPlaying { get; }
        bool FullScreen { get; set; }
        MediaPlayerMode PlayerMode { get; set; }
    }
}
