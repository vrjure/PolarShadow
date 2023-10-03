using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Models
{
    internal class FullScreenState
    {
        public static readonly FullScreenState FullScreen = new FullScreenState { IsFullScreen = true };
        public static readonly FullScreenState Normal = new FullScreenState { IsFullScreen = false };
        public bool IsFullScreen { get; set; }
    }
}
