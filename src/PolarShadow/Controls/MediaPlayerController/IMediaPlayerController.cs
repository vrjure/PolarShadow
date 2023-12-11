using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    internal interface IMediaPlayerController
    {
        string Title { get; set; }
        TimeSpan Length { get; }
        TimeSpan Time { get; }
        bool IsPlaying { get; }
        bool FullScreen { get; set; }
    }
}
