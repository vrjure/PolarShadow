using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IRequestHandler
    {
        Task ExecuteAsync(Stream output, AnalysisRequest request, AnalysisResponse response, IContentBuilder requestBuilder, IContentBuilder responseBuilder, IParameter input, CancellationToken cancellation = default);
    }
}
