using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class WebAnalysisAbleDefault: AnalysisAbilityBase<WebAnalysisSourceFilter, WebAnalysisSource>, IWebAnalysisAble
    {
        public override string Name => Abilities.WebAnalysisAble;
    }
}
