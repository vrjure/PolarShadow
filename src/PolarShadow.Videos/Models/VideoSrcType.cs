using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Videos
{
    [Flags]
    public enum VideoSrcType
    {
        None = 0,
        Http = 1,
        Magnet = 2,
        BaiDu = 4,
        ALiYunDrive = 8,
        Quark = 16,
        M3U8 = 32,
        HTML = 64,
        HTMLAnalysis = 128
    }
}
