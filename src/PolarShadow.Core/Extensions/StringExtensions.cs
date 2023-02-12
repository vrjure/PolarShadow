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
                else if (uri.Host.Equals("www.aliyundrive.com", StringComparison.OrdinalIgnoreCase))
                {
                    return SrcType.ALiYunDrive;
                }
            }

            return SrcType.None;
        }

        public static string CamelCaseName(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            if (name.Length > 1)
            {
                return $"{char.ToLowerInvariant(name[0])}{name.Substring(1)}";
            }
            else
            {
                return name.ToLowerInvariant();
            }
        }

        public static string FormatUrl(this string url, params KeyValuePair<string, string>[] paramaters)
        {
            var span = url.AsSpan();
            var sb = new StringBuilder(url.Length);
            var start = 0;
            var end = 0;
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == '{')
                {
                    start = i;
                    continue;
                }
                else if (span[i] == '}')
                {
                    end = i;

                    if (start - end <= 1)
                    {
                        continue;
                    }
                    var pName = span.Slice(start, end - start);
                    for (int j = 0; j < paramaters.Length; j++)
                    {
                        if (paramaters[i].Key.Equals(new string(pName), StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append(paramaters[i].Value);
                            break;
                        }
                    }
                    continue;
                }

                sb.Append(span[i]);
            }

            return sb.ToString();
        }
    }
}
