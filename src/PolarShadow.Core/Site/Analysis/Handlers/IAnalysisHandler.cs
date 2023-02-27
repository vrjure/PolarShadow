using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface IAnalysisHandler<TInput>
    {
        void Analysis(TInput obj, Stream stream, IReadOnlyDictionary<string, AnalysisAction> actions, JsonElement param);
        T Analysis<T>(TInput obj, IReadOnlyDictionary<string, AnalysisAction> actions, JsonElement param);
    }
}
