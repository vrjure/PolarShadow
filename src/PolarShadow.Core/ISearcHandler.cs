using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface ISearcHandler
    {
        void Reset();
        Task<ICollection<VideoSummary>> SearchNextAsync(CancellationToken cancellation = default);
    }
}
