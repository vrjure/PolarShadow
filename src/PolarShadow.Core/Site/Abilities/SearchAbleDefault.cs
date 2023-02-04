using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PolarShadow.Core
{
    internal class SearchAbleDefault : AnalysisAbilityBase, ISearchAble
    {
        public override string Name => Abilities.SearchAble;

        public async Task<PageResult<VideoSummary>> ExecuteAsync(AnalysisAbility ability, SearchVideoFilter input, CancellationToken cancellation = default)
        {
            var result = await ExecuteAsync<SearchVideoFilter, PageResult<VideoSummary>>(ability, input, cancellation);
            result.Page = input.Page;
            result.PageSize = input.PageSize;
            return result;
        }
    }
}
