using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface ISiteRequest
    {
        public AnalysisRequest Request { get;}
        public AnalysisResponse Response { get;}
        public IKeyValueParameter Parameter { get; }
        public ISiteRequest Next { get; }
        void Write(Utf8JsonWriter writer);
    }
}
