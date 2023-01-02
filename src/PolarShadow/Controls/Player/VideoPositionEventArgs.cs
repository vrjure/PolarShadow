using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    class VideoPositionEventArgs : EventArgs
    {
        public TimeSpan Position { get; private set; }

        public VideoPositionEventArgs(TimeSpan position)
        {
            Position = position;
        }
    }
}
