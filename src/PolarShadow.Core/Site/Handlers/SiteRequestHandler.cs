using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class SiteRequestHandler : ISiteRequestHandler
    {
        private readonly IRequestHandler _handler;
        private readonly SiteRequest _request;
        private readonly Parameters _parameters;
        public SiteRequestHandler(IRequestHandler requesthandler, SiteRequest request, IParameter parameter)
        {
            _handler = requesthandler;
            _request = request;
            _parameters = new Parameters(parameter);
            if (request.Parameter != null)
            {
                _parameters.Add(request.Parameter);
            }
        }

        public async Task ExecuteAsync(Stream stream, CancellationToken cancellation = default)
        {
            await ExecuteAsync(default, stream, cancellation);
        }

        public async Task ExecuteAsync(string input, Stream stream, CancellationToken cancellation = default)
        {
            var p = new Parameters(_parameters);
            if (!string.IsNullOrEmpty(input))
            {
                using var doc = JsonDocument.Parse(input);
                var objectParameter = new ObjectParameter(new ParameterValue(doc.RootElement.Clone()));
                p.Add(objectParameter);
            }
            await _handler.ExecuteAsync(stream, _request.Request, _request.Response, p, cancellation);
        }

        public bool TryGetParameter<T>(string name, out T value)
        {
            return _parameters.TryReadValue(name, out value);
        }
    }
}
