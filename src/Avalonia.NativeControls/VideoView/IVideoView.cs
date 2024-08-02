using LibVLCSharp.Shared;
using PolarShadow.Essentials;
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
    }
}
