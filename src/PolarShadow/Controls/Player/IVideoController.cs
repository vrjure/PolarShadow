using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    interface IVideoController
    {
        PlayerStatus Status { get; set; }
        TimeSpan Duration { get; set; }
    }
}
