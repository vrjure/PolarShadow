using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IDownloadAble
    {
        SrcType DownloadType { get; }
    }
}
