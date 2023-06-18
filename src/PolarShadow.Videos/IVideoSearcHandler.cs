using System;
using System.Collections.Generic;
using System.Text;
using PolarShadow.Core;

namespace PolarShadow.Videos
{
    public interface IVideoSearcHandler : ISequentialRequest<PageResult<VideoSummary>>
    {

    }
}
