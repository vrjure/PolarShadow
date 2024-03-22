using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using PolarShadow.Core;

namespace PolarShadow.Resources
{
    public interface ISiteRequestHandler
    {
        bool TryGetParameter<T>(string name, out T value);
        Task ExecuteAsync(Stream output, Action<IParameterCollection> parameters, CancellationToken cancellation = default);
    }
}
