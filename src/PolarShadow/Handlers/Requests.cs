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

        public static ISearchHandler<TLink> CreateSearchHandler<TLink>(this IPolarShadow polar, string searchText, int maxTaskCount = 3) where TLink : class, ILink
        {
            return new SearchBatchHandler<TLink>(polar, Search, searchText, maxTaskCount);
        }

        public static ISearchHandler<TLink> CreateSearchHandler<TLink>(this IPolarShadow polar, string searchText, IEnumerable<ISite> sites, int maxTaskCount = 3) where TLink : class, ILink
        {
            return new SearchBatchHandler<TLink>(polar,sites, Search, searchText, maxTaskCount);
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
