using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    [Flags]
    public enum LinkType
    {
        None = 0,
        Magnet = 1,
        M3U8 = 2,
        HTML = 4,
        Meida = 8,
        WebAnalysis = 16
    }
}
