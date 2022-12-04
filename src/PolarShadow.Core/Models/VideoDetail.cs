using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class VideoDetail : VideoSummary
    {
        /// <summary>
        /// 视频集数信息
        /// </summary>
        public ICollection<VideoEpisode> Episodes { get; set; }
    }
}
