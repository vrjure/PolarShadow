using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public class RequestRule
    {
        public RequestRule(string requestName)
        {
            if (string.IsNullOrEmpty(requestName)) throw new ArgumentException("requestName must be not empty", nameof(requestName));
            this.RequestName = requestName;
        }
        public string RequestName { get; }
        public string NextRequst { get; set; }
        public ICollection<IContentWriting> Writings { get; set; }

        public static bool MatchWithWildcard(string input, string wildcardString)
        {
            //*_abc_*_*_abc_*
            if (wildcardString == null || wildcardString == null) return false;
            if (wildcardString == "*") return true;

            var buffer_ws = wildcardString.AsSpan();
            var buffer = input.AsSpan();

            var seg_start = 0;
            var seg_end = 0;

            while (true)
            {
                var index = buffer_ws.IndexOf('*');
                if (index == 0)
                {
                    if (buffer_ws.Length > 1)
                    {
                        seg_start = 1;
                        index = buffer_ws.Slice(1).IndexOf('*');
                        if (index < 0)
                        {
                            seg_end = buffer_ws.Length;
                        }
                        else
                        {
                            seg_end = index + 1;
                        }
                    }
                }
                else if (index > 0)
                {
                    seg_start = 0;
                    seg_end = index;
                }
                else
                {
                    seg_end = buffer_ws.Length;
                }

                if (seg_start == seg_end)
                {
                    return true;
                }

                var match_part = buffer_ws.Slice(seg_start, seg_end - seg_start);

                if (seg_start == 0)//just end with '*' or not end with '*'
                {
                    if (!buffer.StartsWith(match_part))
                    {
                        return false;
                    }

                    if (match_part.Length >= buffer.Length)//buffer is end
                    {
                        return true;
                    }

                    if (seg_end >= buffer_ws.Length)//not end with '*'
                    {
                        return false;
                    }

                    buffer = buffer.Slice(match_part.Length);
                    buffer_ws = buffer_ws.Slice(seg_end);
                    seg_start = seg_end = 0;
                }
                else if (seg_start == 1) //start with '*'
                {
                    var findIndex = buffer.IndexOf(match_part);
                    if (findIndex < 0)
                    {
                        return false;
                    }

                    if (findIndex + match_part.Length >= buffer.Length)//buffer is end
                    {
                        return true;
                    }

                    if (seg_end >= buffer_ws.Length) //not end with '*'
                    {
                        return false;
                    }

                    buffer = buffer.Slice(findIndex + match_part.Length);
                    buffer_ws = buffer_ws.Slice(seg_end);
                    seg_start = seg_end = 0;
                }
            }
        }
    }
}
