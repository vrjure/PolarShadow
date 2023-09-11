using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Aria2
{
    public enum Aria2DownloadStatus
    {
        active = 0,
        waiting,
        download,
        paused,
        error,
        complete,
        removed
    }
}
