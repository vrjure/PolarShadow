using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PolarShadow.Core
{
    public interface ISiteRequestHandler
    {
        bool TryGetParameter<T>(string name, out T value);
        Task ExecuteAsync(Stream stream, CancellationToken cancellation = default);
        Task ExecuteAsync(string input, Stream stream, CancellationToken cancellation = default);
    }
}
