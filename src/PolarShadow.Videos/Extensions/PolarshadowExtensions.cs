using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Videos
{
    public static class PolarshadowExtensions
    {
        public static IVideoSearcHandler CreateVideoSearcHandler(this IPolarShadow polar, SearchVideoFilter videoFilter, CancellationToken cancellation = default)
        {
            var sites = polar.GetSites().Where(f => f.HasAbility(VideoAbilities.Search));
            return new VideoSearcHandler(VideoAbilities.Search, videoFilter, sites, polar.Parameters);
        }

        public static async Task<VideoDetail> GetDetailAsync(this IPolarShadowSite site, VideoSummary summary, CancellationToken cancellation = default)
        {
            var result = await site.ExecuteAsync<VideoSummary, VideoDetail>(VideoAbilities.Detail, summary, cancellation);
            if (result == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(result.Name))
            {
                result.Name = summary.Name;
            }
            if (string.IsNullOrEmpty(result.Description))
            {
                result.Description = summary.Description;
            }
            if (string.IsNullOrEmpty(result.ImageSrc))
            {
                result.ImageSrc = summary.ImageSrc;
            }
            if (string.IsNullOrEmpty(result.SiteName))
            {
                result.SiteName = summary.SiteName;
            }
            if (string.IsNullOrEmpty(result.DetailSrc))
            {
                result.DetailSrc = summary.DetailSrc;
            }
            return result;
        }
    }
}
