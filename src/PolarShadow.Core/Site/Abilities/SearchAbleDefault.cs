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
            var result = new PageResult<VideoSummary>();
            result.Page = filter.Page;
            result.PageSize = filter.PageSize;

            var url = AbilityConfig.Url.FormatUrl(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>(nameof(filter.Page), filter.Page.ToString()),
                new KeyValuePair<string, string>(nameof(filter.SearchKey), HttpUtility.UrlEncode(filter.SearchKey.ToString())),
                new KeyValuePair<string, string>(nameof(filter.PageSize), filter.PageSize.ToString())
            });

            //var result = await HandleValueAsync<PageResult<VideoSummary>>(url);
            
            return result;
        }
    }
}
