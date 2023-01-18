using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IAnalysisHandler
    {
        T Analysis<T>(object obj, IReadOnlyDictionary<string, AnalysisAction> actions) where T : class;
    }

    public interface IAnalysisHandler<TInput> : IAnalysisHandler
    {
        T Analysis<T>(TInput obj, IReadOnlyDictionary<string, AnalysisAction> actions) where T : class;
    }
}
