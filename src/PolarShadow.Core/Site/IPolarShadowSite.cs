using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPolarShadowSite
    {
        string Name { get; }
        string Domain { get; }
        Task<VideoDetail> GetVideoDetailAsync(string detailSrc, VideoSummary summary = default);
    }
}
