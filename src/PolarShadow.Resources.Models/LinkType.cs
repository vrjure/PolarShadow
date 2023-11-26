using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public enum LinkType
    {
        None = 0,
        /// <summary>
        /// 是个磁力链接源，用于下载
        /// </summary>
        Magnet = 1,
        /// <summary>
        /// 链接是个可以播放的视频源
        /// </summary>
        Video = 2,
        /// <summary>
        /// 链接是个网页并且可以解析成源
        /// </summary>
        HtmlSource = 3,
        /// <summary>
        /// 链接可以使用web解析成源
        /// </summary>
        WebAnalysisSource = 4,

    }
}
