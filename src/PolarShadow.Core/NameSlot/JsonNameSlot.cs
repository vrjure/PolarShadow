using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class JsonNameSlot
    {
        public static string NameSlot(this string nameSlotString, JsonElement element)
        {
            return NameSlot(Encoding.UTF8.GetBytes(nameSlotString),element);
        }

        public static string NameSlot(this ReadOnlySpan<byte> span, JsonElement element)
        {
            if (span.Length == 0)
            {
                return span.ToString();
            }

            var sb = new StringBuilder(span.Length);

            var startIndex = -1;
            var endIndex = -1;

            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == '{')
                {
                    startIndex = i;
                    continue;
                }
                else if (span[i] == '}')
                {
                    endIndex = i;

                    if (endIndex - startIndex <= 1)
                    {
                        continue;
                    }
                    var p = span.Slice(startIndex + 1, endIndex - startIndex - 1);



                    startIndex = endIndex;
                    continue;
                }

                if (startIndex <= endIndex)
                {
                    sb.Append((char)span[i]);
                }
            }

            return sb.ToString();
        }
    }
}
