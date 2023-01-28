using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class GetDetailAbleDetail : AnalysisAbilityHandler, IGetDetailAble
    {
        public GetDetailAbleDetail(AnalysisAbility ability) : base(ability)
        {
        }

        public async Task<VideoDetail> GetVideoDetailAsync(VideoSummary summary)
        {
            return await HandleValueAsync<VideoSummary, VideoDetail>(summary);
        }
    }
}
