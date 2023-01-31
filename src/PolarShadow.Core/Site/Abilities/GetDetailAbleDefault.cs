using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class GetDetailAbleDefault : AnalysisAbilityHandler, IGetDetailAble
    {
        public GetDetailAbleDefault(AnalysisAbility ability) : base(ability)
        {
        }

        public async Task<VideoDetail> GetVideoDetailAsync(VideoSummary summary)
        {
            var detail = await HandleValueAsync<VideoSummary, VideoDetail>(summary);
            if (string.IsNullOrEmpty(detail.Description))
            {
                detail.Description = summary.Description;
            }
            if (string.IsNullOrEmpty(detail.Name))
            {
                detail.Name = summary.Name;

            }
            if (string.IsNullOrEmpty(detail.ImageSrc))
            {
                detail.ImageSrc = summary.ImageSrc;
            }
            if (string.IsNullOrEmpty(detail.SiteName))
            {
                detail.SiteName = summary.SiteName;
            }
            if (string.IsNullOrEmpty(detail.DetailSrc))
            {
                detail.DetailSrc = summary.DetailSrc;
            }
            return detail;
        }
    }
}
