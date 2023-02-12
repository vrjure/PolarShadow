using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class GetDetailAbleDefault : AnalysisAbilityBase<VideoSummary, VideoDetail>, IGetDetailAble
    {
        public override string Name => Abilities.GetDetailAble;

        protected override void ValueHandler(VideoSummary input, VideoDetail output)
        {
            if (string.IsNullOrEmpty(output.Description))
            {
                output.Description = input.Description;
            }
            if (string.IsNullOrEmpty(output.Name))
            {
                output.Name = input.Name;

            }
            if (string.IsNullOrEmpty(output.ImageSrc))
            {
                output.ImageSrc = input.ImageSrc;
            }
            if (string.IsNullOrEmpty(output.SiteName))
            {
                output.SiteName = input.SiteName;
            }
            if (string.IsNullOrEmpty(output.DetailSrc))
            {
                output.DetailSrc = input.DetailSrc;
            }
        }
    }
}
