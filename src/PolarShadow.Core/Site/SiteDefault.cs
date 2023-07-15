﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    internal class SiteDefault : ISite
    {
        [JsonIgnore]
        public ISiteRequest this[string requestName]
        {
            get
            {
                if (string.IsNullOrEmpty(requestName)
                    || _requests == null
                    || !_requests.ContainsKey(requestName))
                    return null;

                return _requests[requestName];
            }
            set
            {
                if (string.IsNullOrEmpty(requestName)) return;
                _requests ??= new Dictionary<string, ISiteRequest>();
                _requests[requestName] = value;
            }
        }

        [JsonRequired]
        public string Name { get; set; }

        public string Domain { get; set; }
        public bool UseWebView { get; set; }
        public IKeyValueParameter Parameters { get; set; }

        [JsonPropertyName("requests")]
        public Dictionary<string, ISiteRequest> _requests { get; set; }

        [JsonIgnore]
        public IEnumerable<ISiteRequest> Requests => _requests?.Values;

        [JsonIgnore]
        internal IRequestHandler RequestHandlerInternal { get; set; }
        [JsonIgnore]
        internal IParameterCollection ParametersInternal { get; set; }

        public ISiteRequestHandler CreateRequestHandler(string requestName)
        {
            if (_requests == null) return null;
            if (RequestHandlerInternal == null) throw new InvalidOperationException("RequestHandler not be set");
            if (_requests.TryGetValue(requestName, out ISiteRequest request))
            {
                return new SiteRequestHandler(RequestHandlerInternal, request, ParametersInternal);
            }
            return null;
        }

        public void Remove(string requestName)
        {
            _requests?.Remove(requestName);
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            JsonSerializer.Serialize(writer, this, JsonOption.DefaultSerializer);
        }
    }
}