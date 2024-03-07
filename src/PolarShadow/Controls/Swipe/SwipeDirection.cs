using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    [Flags]
    public enum SwipeDirection
    {
        None = 0,
        TopToBottom = 1,
        BottomToTop = 2,
        LeftToRight = 4,
        RightToLeft = 8
    }
}
