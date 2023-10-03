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

        public static ISearchHandler<TLink> CreateSearchHander<TLink>(this IPolarShadow polarShadow, SearchFilter filter) where TLink : class, ILink
        {
            var sites = polarShadow.GetSites(f => f.HasRequest(Search));
            return new SearchSequentialRequest<TLink>(Search, filter, sites);
        }

        public static async Task<ResourceTree> GetDetailAsync(this ISite site, ILink link, CancellationToken cancellation = default)
        {
            var result = await site.ExecuteAsync<ILink, ResourceTree>(Detail, link, cancellation);
            if (result == null)
            {
                return result;
            }
            return result;
        }
    }
}
