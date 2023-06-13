using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface ISequentialRequest
    {
        IPolarShadowSite Current { get; }
        void Reset();
        Task SearchNextAsync(Stream stream, CancellationToken cancellation = default);
    }

    public interface ISequentialRequest<TOutput> : ISequentialRequest
    {
        Task<TOutput> SearchNextAsync(CancellationToken cancellation = default);
    }
}
