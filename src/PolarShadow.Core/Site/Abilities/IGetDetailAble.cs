using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IGetDetailAble
    {
        Task<VideoDetail> GetVideoDetailAsync(VideoSummary summary);
    }
}
