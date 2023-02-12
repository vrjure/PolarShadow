using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class AnalysisSourceAbleDefault: AnalysisAbilityBase<WebAnalysisSourceFilter, WebAnalysisSource>, IAnalysisSourceAble
    {
        public override string Name => Abilities.WebAnalysisAble;
    }
}
