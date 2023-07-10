using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface ISiteRequest
    {
        public AnalysisRequest Request { get;}
        public AnalysisResponse Response { get;}
        public IParameter Parameter { get; }
        public ISiteRequest Next { get; }
    }
}
