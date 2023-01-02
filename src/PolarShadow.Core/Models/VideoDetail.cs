using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class VideoDetail : VideoSummary
    {
        public VideoDetail() { }

        public VideoDetail(VideoSummary summary)
        {
            if (summary == null)
            {
                return;
            }
            this.Description = summary.Description;
            this.DetailSrc = summary.DetailSrc;
            this.ImageSrc = summary.ImageSrc;
            this.SiteName = summary.SiteName;
            this.Name = summary.Name;

            Episodes = new VideoEpisodeCollection(summary);
        }
        /// <summary>
        /// 视频集数信息
        /// </summary>
        public ICollection<VideoEpisode> Episodes { get; }
    }
}
