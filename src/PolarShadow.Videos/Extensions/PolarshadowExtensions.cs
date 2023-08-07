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
        public static IVideoSearcHandler CreateVideoSearcHandler(this IPolarShadow polarShadow, SearchVideoFilter videoFilter)
        {
            var sites = polarShadow.GetSites(f => f.HasRequest(VideoRequests.Search));
            return new VideoSearcHandler(VideoRequests.Search, videoFilter, sites);
        }

        public static ISequentialRequest<ICollection<VideoSummary>> CreateVideoDiscoverHandler(this IPolarShadow polarShadow)
        {
            var sites = polarShadow.GetSites(f => f.HasRequest(VideoRequests.Main));
            return new VideoDiscoverHandler(VideoRequests.Main, sites);
        }

        public static async Task<VideoDetail> GetDetailAsync(this ISite site, ILink link, CancellationToken cancellation = default)
        {
            var result = await site.ExecuteAsync<ILink, VideoDetail>(VideoRequests.Detail, link, cancellation);
            if (result == null)
            {
                return result;
            }

            if (link is VideoSummary summary)
            {
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
            }
            
            return result;
        } 

        public static IEnumerable<WebAnalysisSource> GetAnalysisSources(this IPolarShadow polarShadow)
        {
            return polarShadow.GetItem<IWebAnalysisItem>()?.Sources;
        }
    }
}
