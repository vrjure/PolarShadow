using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PolarShadow.Core
{
    internal class SearchAbleDefault : AnalysisAbilityBase<SearchVideoFilter, PageResult<VideoSummary>>, ISearchAble
    {
        public override string Name => Abilities.SearchAble;

        protected override void HandleInput(SearchVideoFilter input)
        {
            input.SearchKey = HttpUtility.UrlEncode(input.SearchKey);
        }

        protected override void ValueHandler(SearchVideoFilter input, PageResult<VideoSummary> output)
        {
            output.Page = input.Page;
        }
    }
}
