using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace PolarShadow.Core
{
    public interface IAnalysisAbilityBuilder
    {
        IAnalysisAbilityBuilder SetRequest(AnalysisRequest request);
        IAnalysisAbilityBuilder SetResponse(AnalysisResponse response);
        IAnalysisAbilityBuilder Next();
        IParametersBuilder Parameters { get; }
        void WriteTo(Stream output);
        void Load(string config);
    }

}
