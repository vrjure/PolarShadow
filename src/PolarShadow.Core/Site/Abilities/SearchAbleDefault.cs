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
        private static readonly string _pageingFlag = "searchPaging";
        public override string Name => Abilities.SearchAble;

        public bool CanPaging(IPolarShadowSite site)
        {
            if (site.TryGetParameter(_pageingFlag, out bool value))
            {
                return value;
            }

            return true;
        }

        protected override void InputHandler(SearchVideoFilter input)
        {
            input.SearchKey = HttpUtility.UrlEncode(input.SearchKey);
        }

        protected override void ValueHandler(SearchVideoFilter input, PageResult<VideoSummary> output)
        {
            output.Page = input.Page;
        }
    }
}
