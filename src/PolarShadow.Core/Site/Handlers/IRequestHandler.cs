using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IRequestHandler
    {
        Task ExecuteAsync(Stream output, AnalysisRequest request, AnalysisResponse response, IParameter input, CancellationToken cancellation = default);
    }
}
