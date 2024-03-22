using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Resources
{
    public interface ISequentialRequest : IEnumerator<ISite>
    {
        Task ExecuteAsync(Stream stream, CancellationToken cancellation = default);
    }

    public interface ISequentialRequest<TOutput> : ISequentialRequest
    {
        Task<TOutput> ExecuteAsync(CancellationToken cancellation = default);
    }
}
