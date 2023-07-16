using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public class JsonStreamProvider : JsonProvider
    {
        private readonly JsonStreamSource _source;

        public JsonStreamProvider(JsonStreamSource source)
        {
            _source = source;
        }

        protected override JsonElement Parse()
        {
            if (_source.Stream == null)
            {
                return default;
            }

            using var doc = JsonDocument.Parse(_source.Stream);
            return doc.RootElement.Clone();
        }
    }
}
