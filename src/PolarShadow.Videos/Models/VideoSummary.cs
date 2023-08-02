using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Videos
{
    public class VideoSummary : Link
    {
        /// <summary>
        /// 摘要
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 封面地址
        /// </summary>
        public string ImageSrc { get; set; }
    }
}
