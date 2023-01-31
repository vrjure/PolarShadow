using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public static class StringExtensions
    {
        public static SrcType GetVideoSourceType(this string src)
        {
            if (src == null)
            {
                return SrcType.None;
            }
            if (src.StartsWith("magnet:"))
            {
                return SrcType.Magnet;
            }
            else if (src.StartsWith("http"))
            {
                var uri = new Uri(src);
                if (uri.Host.Equals("pan.quark.cn", StringComparison.OrdinalIgnoreCase))
                {
                    return SrcType.Quark;
                }
                else if (uri.Host.Equals("pan.baidu.com"))
                {
                    return SrcType.BaiDu;
                }
            }

            return SrcType.None;
        }
    }
}
