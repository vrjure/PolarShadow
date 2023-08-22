using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Views
{
    internal interface IMediaController
    {
        string Title { get; set; }
        int Volume { get; set; }
        TimeSpan Time { get; set; }
        void Play();
        void Pause();
    }
}
