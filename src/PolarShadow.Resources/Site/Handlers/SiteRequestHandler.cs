using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PolarShadow.Core;

namespace PolarShadow.Resources
{
    internal class SiteRequestHandler : ContentWriter, ISiteRequestHandler
    {
        private readonly IRequestHandler _handler;
        private readonly ISiteRequest _request;
        private readonly IParameter _parameters;
        private readonly IEnumerable<IContentWriting> _writingCollection;

        private IEnumerable<IContentWriting> _writings;
        public SiteRequestHandler(IRequestHandler requesthandler, ISiteRequest request, IParameter parameter, IEnumerable<IContentWriting> writingCollection)
        {
            _handler = requesthandler;
            _request = request;
            _parameters = parameter;
            _writingCollection = writingCollection;
        }

        public async Task ExecuteAsync(Stream output, Action<IParameterCollection> parametersBuilder, CancellationToken cancellation = default)
        {
            if (_request.Response == null || !_request.Response.Template.HasValue)
            {
                return;
            }

            var p = new Parameters(_parameters);
            parametersBuilder?.Invoke(p);

            var result = await _handler.ExecuteAsync(_request, p, cancellation);

            if (result != null)
            {
                p.Add(result);
            }

            _writings = _writingCollection?.Select(f => TryGetClone(f)).ToList();

            this.Build(output, _request.Response.Template.Value, p);

#if DEBUG
            try
            {
                output.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(output);
                System.Diagnostics.Debug.WriteLine(sr.ReadToEnd());
                output.Seek(0, SeekOrigin.Begin);

            }
            catch { }
#endif
        }

        public bool TryGetParameter<T>(string name, out T value)
        {
            return _parameters.TryReadValue(name, out value);
        }

        protected override bool BeforeWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            if (_writings == null) return false;

            var result = false;
            foreach (var item in _writings) 
            { 
                result = result || item.BeforeWriteProperty(writer, property, parameter); 
            }

            return result;
        }

        protected override void AfterWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            if (_writings == null) return;
            foreach (var item in _writings) { item.AfterWriteProperty(writer, property, parameter); }
        }


        protected override void BeforeWriteStartObject(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if(_writings == null) return;
            foreach (var item in _writings) { item.BeforeWriteStartObject(writer, propertyName, parameter); }
        }

        protected override void AfterWriteStartObject(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (_writings == null) return;
            foreach (var item in _writings) { item.AfterWriteStartObject(writer, propertyName, parameter); }
        }

        protected override void BeforeWriteEndObject(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (_writings == null) return;
            foreach(var item in _writings) { item.BeforeWriteEndObject(writer, propertyName, parameter); }
        }

        protected override void AfterWriteEndObject(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (_writings == null) return;
            foreach(var item in _writings) { item.AfterWriteEndObject(writer, propertyName, parameter); }
        }


        protected override void BeforeWriteStartArray(Utf8JsonWriter writer, string property, IParameter parameter)
        {
            if (_writings == null) return;
            foreach (var item in _writings) { item.BeforeWriteStartArray(writer, property, parameter); }
        }

        protected override void AfterWriteStartArray(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (_writings == null) return;
            foreach(var item in _writings) { item.AfterWriteStartArray(writer, propertyName, parameter); }
        }


        protected override void BeforeWriteEndArray(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (_writings == null) return;
            foreach(var item in _writings) { item.BeforeWriteEndArray(writer, propertyName, parameter); }
        }

        protected override void AfterWriteEndArray(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (writer == null) return;
            foreach(var item in _writings) { item.AfterWriteEndArray(writer, propertyName, parameter); }
        }

        private IContentWriting TryGetClone(IContentWriting writing)
        {
            if (writing is ICloneable clone)
            {
                return (IContentWriting)clone.Clone();
            }
            return writing;
        }
    }
}
