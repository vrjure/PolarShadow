using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PolarShadow.Core;

namespace PolarShadow.Videos
{
    public interface IVideoSearcHandler : ISequentialRequest<PageResult<VideoSummary>>
    {
        Task<PageResult<VideoSummary>> SearchNextAsync(CancellationToken cancellation = default);
    }
}
