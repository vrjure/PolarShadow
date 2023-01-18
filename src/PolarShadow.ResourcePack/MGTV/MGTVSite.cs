using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.ResourcePack
{
    public class MGTVSite
    {
        public string Name => "MGTV";

        public string Domain => "www.mgtv.com";

        public Task<PageResult<VideoSummary>> SearchVideosAsync(SearchVideoFilter filter, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }
    }
}
