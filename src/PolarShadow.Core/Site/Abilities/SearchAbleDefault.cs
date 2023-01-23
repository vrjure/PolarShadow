using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PolarShadow.Core
{
    internal class SearchAbleDefault : AnalysisAbilityHandler, ISearchAble
    {
        public SearchAbleDefault(AnalysisAbility ability) : base(ability)
        {
            
        }

        public async Task<PageResult<VideoSummary>> SearchVideosAsync(SearchVideoFilter filter, CancellationToken cancellation = default)
        {
            var result = await HandleValueAsync<SearchVideoFilter , PageResult<VideoSummary>>(filter);
            result.Page = filter.Page;
            result.PageSize = filter.PageSize;
            return result;
        }
    }
}
