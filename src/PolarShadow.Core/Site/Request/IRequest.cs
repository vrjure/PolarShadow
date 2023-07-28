using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IRequest
    {
        AnalysisRequest Request { get; }
        AnalysisResponse Response { get; }
    }
}
