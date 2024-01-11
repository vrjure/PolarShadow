using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public interface IVideoView
    {
        IVideoViewController Controller { get; set; }
        bool FullScreen { get; set; }

        event EventHandler PlatformClick;
    }
}
