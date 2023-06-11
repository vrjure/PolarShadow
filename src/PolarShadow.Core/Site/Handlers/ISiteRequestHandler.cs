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
        Task ExecuteAsync(string name, string input, Stream stream, CancellationToken cancellation = default);
    }
}
