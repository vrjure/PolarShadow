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
        private readonly ISite _site;
        private readonly IRequestHandler _handler;
        private readonly ISiteRequest _request;
        private readonly IParameterCollection _parameters;
        private readonly IContentBuilder _requestBuilder;
        private readonly IContentBuilder _responseBuilder;
        public SiteRequestHandler(ISite site, IRequestHandler requesthandler, ISiteRequest request, IParameter parameter, IContentBuilder requestBuilder, IContentBuilder responseBuilder)
        {
            _site = site;
            _handler = requesthandler;
            _request = request;
            _parameters = new Parameters(parameter);
            if (request.Parameter != null)
            {
                _parameters.Add(request.Parameter);
            }

            var siteInfo = new KeyValueParameter
            {
                { $"site:name", _site.Name },
                { $"site:domain", _site.Domain }
            };
            _parameters.Add(siteInfo);

            _requestBuilder = requestBuilder;
            _responseBuilder = responseBuilder;
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
            await _handler.ExecuteAsync(stream, _request.Request, _request.Response, _requestBuilder, _responseBuilder, p, cancellation);
        }

        public bool TryGetParameter<T>(string name, out T value)
        {
            return _parameters.TryReadValue(name, out value);
        }
    }
}
