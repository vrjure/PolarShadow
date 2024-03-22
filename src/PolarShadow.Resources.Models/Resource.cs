using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public class Resource : Link
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string ImageSrc { get; set; }
        /// <summary>
        /// 图片访问请求Header
        /// </summary>
        public IDictionary<string, string> ImageSrcHeaders { get; set; }
    }
}
