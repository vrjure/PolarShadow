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
        public static IVideoSearcHandler CreateVideoSearcHandler(this IPolarShadow polarShadow, SearchVideoFilter videoFilter, CancellationToken cancellation = default)
        {
            var sites = polarShadow.GetSites(f => f.HasRequest(VideoRequests.Search));
            return new VideoSearcHandler(VideoRequests.Search, videoFilter, sites);
        }

        public static ISequentialRequest<ICollection<VideoSummary>> CreateVideoDiscoverHandler(this IPolarShadow polarShadow, CancellationToken cancellation= default)
        {
            var sites = polarShadow.GetSites(f => f.HasRequest(VideoRequests.Newest));
            return new VideoDiscoverHandler(VideoRequests.Newest, sites);
        }

        public static async Task<VideoDetail> GetDetailAsync(this ISite site, VideoSummary summary, CancellationToken cancellation = default)
        {
            var result = await site.ExecuteAsync<VideoSummary, VideoDetail>(VideoRequests.Detail, summary, cancellation);
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
            if (string.IsNullOrEmpty(result.Site))
            {
                result.Site = summary.Site;
            }
            if (string.IsNullOrEmpty(result.Src))
            {
                result.Src = summary.Src;
            }
            return result;
        } 

        public static IEnumerable<WebAnalysisSource> GetAnalysisSources(this IPolarShadow polarShadow)
        {
            return polarShadow.GetItem<IWebAnalysisItem>()?.Sources;
        }
    }
}
