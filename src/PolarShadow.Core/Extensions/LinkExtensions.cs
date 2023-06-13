using System;
using System.Collections.Generic;
using System.Text;
using PolarShadow.Core.Models;

namespace PolarShadow.Core
{
    public static class LinkExtensions
    {
        public static LinkType GetVideoSourceType(this string link)
        {
            if (link == null)
            {
                return LinkType.None;
            }
            if (link.StartsWith("magnet:"))
            {
                return LinkType.Magnet;
            }
            else if (link.StartsWith("http"))
            {
                if (link.EndsWith(".m3u8"))
                {
                    return LinkType.M3U8;
                }
                var uri = new Uri(link);
                if (uri.Host.Equals("pan.quark.cn", StringComparison.OrdinalIgnoreCase))
                {
                    return LinkType.Quark;
                }
                else if (uri.Host.Equals("pan.baidu.com"))
                {
                    return LinkType.BaiDu;
                }
                else if (uri.Host.Equals("www.aliyundrive.com", StringComparison.OrdinalIgnoreCase))
                {
                    return LinkType.ALiYunDrive;
                }
                else
                {
                    return LinkType.HttpFile;
                }
            }

            return LinkType.None;
        }
    }
}
