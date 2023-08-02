using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class SiteRequestHandler : ContentWriter, ISiteRequestHandler
    {
        private readonly ISite _site;
        private readonly IRequestHandler _handler;
        private readonly ISiteRequest _request;
        private readonly IParameterCollection _parameters;
        private readonly IEnumerable<IContentWriting> _writingCollection;
        public SiteRequestHandler(ISite site, IRequestHandler requesthandler, ISiteRequest request, IParameter parameter, IEnumerable<IContentWriting> writingCollection)
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
                { "site:name", _site.Name },
                { "site:domain", _site.Domain }
            };
            _parameters.Add(siteInfo);
            _writingCollection = writingCollection;
        }

        public async Task ExecuteAsync(string input, Stream output, CancellationToken cancellation = default)
        {
            if (_request.Response == null || !_request.Response.Template.HasValue)
            {
                return;
            }

            var p = new Parameters(_parameters);
            if (!string.IsNullOrEmpty(input))
            {
                using var doc = JsonDocument.Parse(input);
                var objectParameter = new ObjectParameter(new ParameterValue(doc.RootElement.Clone()));
                p.Add(objectParameter);
            }
            var result = await _handler.ExecuteAsync(new RequestInternal
            {
                Request = _request.Request,
                Response = _request.Response
            }, p, cancellation);

            if (result != null)
            {
                p.Add(result);
            }

            this.Write(output, _request.Response.Template.Value, p);
        }

        public bool TryGetParameter<T>(string name, out T value)
        {
            return _parameters.TryReadValue(name, out value);
        }

        protected override bool BeforeWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            if (_writingCollection == null) return false;

            var result = false;
            foreach (var item in _writingCollection) 
            { 
                result = result || item.BeforeWriteProperty(writer, property, parameter); 
            }

            return result;
        }

        protected override void AfterWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            if (_writingCollection == null) return;
            foreach (var item in _writingCollection) { item.AfterWriteProperty(writer, property, parameter); }
        }

        protected override void AfterWriteStartObject(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (_writingCollection == null) return;
            foreach (var item in _writingCollection) { item.AfterWriteStartObject(writer, propertyName, parameter); }
        }

        protected override void BeforeWriteEndObject(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (_writingCollection == null) return;
            foreach(var item in _writingCollection) { item.BeforeWriteEndObject(writer, propertyName, parameter); }
        }

        protected override void AfterWriteStartArray(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (_writingCollection == null) return;
            foreach(var item in _writingCollection) { item.AfterWriteStartArray(writer, propertyName, parameter); }
        }

        protected override void BeforeWriteEndArray(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (_writingCollection == null) return;
            foreach(var item in _writingCollection) { item.BeforeWriteEndArray(writer, propertyName, parameter); }
        }
    }
}
