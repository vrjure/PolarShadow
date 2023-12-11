using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls
{
    public interface IVideoView
    {
        MediaPlayer MediaPlayer { get; set; }
        bool FullScreen { get; set; }
        event EventHandler PlatformClick;
    }
}
