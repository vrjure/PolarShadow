using LibVLCSharp.Shared;
using PolarShadow.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.VLC
{
    public interface IVLController : IVideoViewController
    {
        public MediaPlayer MediaPlayer { get; }
    }
}
