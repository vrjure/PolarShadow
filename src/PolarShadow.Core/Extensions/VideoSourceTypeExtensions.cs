using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public static class VideoSourceTypeExtensions
    {
        public static VideoSourceType GetVideoSourceType(this string src)
        {
            if (src == null)
            {
                return VideoSourceType.None;
            }
            if (src.StartsWith("magnet:"))
            {
                return VideoSourceType.Magnet;
            }
            else if (src.StartsWith("http"))
            {
                var uri = new Uri(src);
                if (uri.Host.Equals("pan.quark.cn", StringComparison.OrdinalIgnoreCase))
                {
                    return VideoSourceType.Quark;
                }
                else if (uri.Host.Equals("pan.baidu.com"))
                {
                    return VideoSourceType.BaiDu;
                }
            }

            return VideoSourceType.None;
        }
    }
}
