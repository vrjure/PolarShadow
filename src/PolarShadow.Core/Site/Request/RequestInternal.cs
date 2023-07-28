using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal class RequestInternal : IRequest
    {
        public AnalysisRequest Request { get; set; }

        public AnalysisResponse Response { get; set; }
    }
}
