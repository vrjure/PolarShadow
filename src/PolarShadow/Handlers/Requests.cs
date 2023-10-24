using PolarShadow.Core;
using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow
{
    public static class Requests
    {
        public static string Search = "search";
        public static string Detail = "detail";
        public const string Main = "main";
        public const string Categories = "categories";
        public const string Video = "video";

        public static ISearchHandler<TLink> CreateSearchHander<TLink>(this IPolarShadow polar, string searchText) where TLink : class, ILink
        {
            return new SearchBatchHandler<TLink>(polar, Search, searchText);
        }

        public static async Task<ResourceTree> GetDetailAsync(this IPolarShadow polar, ISite site, ILink link, CancellationToken cancellation = default)
        {
            var result = await site.ExecuteAsync<ILink, ResourceTree>(polar, Detail, link, cancellation);
            if (result == null)
            {
                return result;
            }
            return result;
        }
    }
}
