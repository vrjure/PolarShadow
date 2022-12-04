﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    [Flags]
    public enum VideoSourceType
    {
        None = 0,
        Http  = 1,
        Magnet = 2,
        BaiDu = 4,
        ALiYun = 8,
        Quark = 16,
        M3U8 = 32
    }
}
