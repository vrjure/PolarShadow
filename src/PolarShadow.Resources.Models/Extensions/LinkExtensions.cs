using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public static class LinkExtensions
    {
        public static LinkType GetLinkType(this string link)
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
                if (link.Split('?')[0].EndsWith(".m3u8"))
                {
                    return LinkType.M3U8;
                }
                else if (link.EndsWith(".html"))
                {
                    return LinkType.HTML;
                }
            }

            return LinkType.None;
        }
    }
}
