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
            detail.Description = summary.Description;
            detail.Name = summary.Name;
            detail.ImageSrc = summary.ImageSrc;
            detail.SiteName = summary.SiteName;
            detail.DetailSrc = summary.DetailSrc;
            return detail;
        }
    }
}
