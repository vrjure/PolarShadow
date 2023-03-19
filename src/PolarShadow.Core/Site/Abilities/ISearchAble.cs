using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface ISearchAble : IAnalysisAbility<SearchVideoFilter, PageResult<VideoSummary>>
    {
        bool CanPaging(IPolarShadowSite site);
    }
}
