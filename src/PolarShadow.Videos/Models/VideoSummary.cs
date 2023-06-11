using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Videos
{
    public class VideoSummary
    {
        public string Name { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 封面地址
        /// </summary>
        public string ImageSrc { get; set; }
        /// <summary>
        /// 详情地址
        /// </summary>
        public string DetailSrc { get; set; }
        /// <summary>
        /// 视频来源
        /// </summary>
        public string SiteName { get; set; }
    }
}
