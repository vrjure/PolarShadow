using PolarShadow.Core;
using PolarShadow.Videos;
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

        public static ISearchHandler CreateSearchHander(this IPolarShadow polarShadow, SearchFilter filter)
        {
            var sites = polarShadow.GetSites(f => f.HasRequest(Search));
            return new SearchSequentialRequest(Search, filter, sites);
        }

        public static async Task<Resource> GetDetailAsync(this ISite site, ILink link, CancellationToken cancellation = default)
        {
            var result = await site.ExecuteAsync<ILink, Resource>(Detail, link, cancellation);
            if (result == null)
            {
                return result;
            }

            if (link is Resource res)
            {
                if (string.IsNullOrEmpty(result.Name))
                {
                    result.Name = res.Name;
                }
                if (string.IsNullOrEmpty(result.Description))
                {
                    result.Description = res.Description;
                }
                if (string.IsNullOrEmpty(result.ImageSrc))
                {
                    result.ImageSrc = res.ImageSrc;
                }
                if (string.IsNullOrEmpty(result.Site))
                {
                    result.Site = res.Site;
                }
                if (string.IsNullOrEmpty(result.Src))
                {
                    result.Src = res.Src;
                }
            }
            return result;
        }
    }
}
