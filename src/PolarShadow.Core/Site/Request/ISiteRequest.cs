using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface ISiteRequest
    {
        public bool? UseWebView { get; }
        public AnalysisRequest Request { get;}
        public AnalysisResponse Response { get;}
        public IKeyValueParameter Parameter { get; }
        void Write(Utf8JsonWriter writer);
    }
}
