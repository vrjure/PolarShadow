using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Videos
{
    public static class PolarshadowExtensions
    {
        public static IEnumerable<WebAnalysisSource> GetAnalysisSources(this IPolarShadow polarShadow)
        {
            return polarShadow.GetItem<IWebAnalysisItem>()?.Sources;
        }
    }
}
