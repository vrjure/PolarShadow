using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class GetDetailAbleDefault : AnalysisAbilityBase, IGetDetailAble
    {
        public override string Name => Abilities.GetDetailAble;

        public async Task<VideoDetail> ExecuteAsync(AnalysisAbility ability, VideoSummary input, CancellationToken cancellation = default)
        {
            var detail = await ExecuteAsync<VideoSummary, VideoDetail>(ability, input, cancellation);
            if (string.IsNullOrEmpty(detail.Description))
            {
                detail.Description = input.Description;
            }
            if (string.IsNullOrEmpty(detail.Name))
            {
                detail.Name = input.Name;

            }
            if (string.IsNullOrEmpty(detail.ImageSrc))
            {
                detail.ImageSrc = input.ImageSrc;
            }
            if (string.IsNullOrEmpty(detail.SiteName))
            {
                detail.SiteName = input.SiteName;
            }
            if (string.IsNullOrEmpty(detail.DetailSrc))
            {
                detail.DetailSrc = input.DetailSrc;
            }
            return detail;
        }
    }
}
