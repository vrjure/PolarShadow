using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface ISiteRequest : IWriterJson
    {
        bool? UseWebView { get; }
        AnalysisRequest Request { get;}
        AnalysisResponse Response { get;}
        IKeyValueParameter Parameter { get; }
    }
}
