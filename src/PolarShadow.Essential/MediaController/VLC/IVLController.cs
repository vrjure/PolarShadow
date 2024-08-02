using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IVLController : IVideoViewController
    {
        public MediaPlayer MediaPlayer { get; }
    }
}
