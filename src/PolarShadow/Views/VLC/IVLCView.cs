using LibVLCSharp.Shared;
using LibVLCSharp.Shared.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Views
{
    internal interface IVLCView : IView
    {
        MediaPlayer MediaPlayer { get; }
    }
}
