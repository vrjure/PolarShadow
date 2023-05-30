using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface ISearcHandler
    {
        void Reset();
        Task SearchNextAsync(string input, Stream stream, CancellationToken cancellation = default);
    }

    public interface ISearcHandler<TInput, TOutput> : ISearcHandler
    {
        Task<TOutput> SearchNextAsync(TInput input, Stream stream, CancellationToken cancellation = default);
    }
}
