using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Videos
{
    public static class StringExtensions
    {
        public static VideoSrcType GetVideoSourceType(this string src)
        {
            if (src == null)
            {
                return VideoSrcType.None;
            }
            if (src.StartsWith("magnet:"))
            {
                return VideoSrcType.Magnet;
            }
            else if (src.StartsWith("http"))
            {
                var uri = new Uri(src);
                if (uri.Host.Equals("pan.quark.cn", StringComparison.OrdinalIgnoreCase))
                {
                    return VideoSrcType.Quark;
                }
                else if (uri.Host.Equals("pan.baidu.com"))
                {
                    return VideoSrcType.BaiDu;
                }
                else if (uri.Host.Equals("www.aliyundrive.com", StringComparison.OrdinalIgnoreCase))
                {
                    return VideoSrcType.ALiYunDrive;
                }
            }

            return VideoSrcType.None;
        }
    }
}
