using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace PolarShadow.Core
{
    public static class NameSlot
    {
        public static string Format(this string text, IParameter value)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return Format(Encoding.UTF8.GetBytes(text), value);
        }

        public static string Format(this ReadOnlySpan<byte> text, IParameter value)
        {
            if (text.IsEmpty)
            {
                return string.Empty;
            }
            var sb = new StringBuilder(text.Length);
            var reader = new NameSlotReader(text);
            Format(sb, ref reader, value);
            return sb.ToString();
        }

        private static void Format(StringBuilder sb, ref NameSlotReader reader, IParameter value)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case NameSlotTokenType.None:
                    case NameSlotTokenType.Start:
                    case NameSlotTokenType.End:
                        break;
                    case NameSlotTokenType.Text:
                        sb.Append(reader.GetString());
                        break;
                    default:
                        ReadValue(sb, ref reader, value);
                        break;
                }
            }

        }

        private static void ReadValue(StringBuilder sb, ref NameSlotReader reader, IParameter value)
        {
            var result = string.Empty;
            if (reader.TokenType == NameSlotTokenType.Parameter)
            {
                result = reader.GetString();
            }

            reader.Read();

            if (value.TryGetValue(result, out ParameterValue newResult))
            {
                result = newResult.GetValue();

                if (reader.TokenType == NameSlotTokenType.Format)
                {
                    var format = reader.GetSegment();
                    result = FormatValue(result, format);
                }
                else if (reader.TokenType == NameSlotTokenType.Match)
                {
                    var regex = reader.GetString();
                    result = MatchValue(result, regex);
                }

            }
                   
            sb.Append(result);
        }

        private static string FormatValue(string value, ReadOnlySpan<byte> format)
        {
            if (NameSlotConstants.UrlEncode.SequenceEqual(format))
            {
               return HttpUtility.UrlEncode(value);
            }

            var formatStr = Encoding.UTF8.GetString(format);
            if (NameSlotConstants.NumberFormatCommonChars.IndexOf(format[0]) >= 0)
            {
                return string.Format($"{{0:{formatStr}}}", Convert.ToDecimal(value));
            }
            else if (NameSlotConstants.NumberFormatIntegralChars.IndexOf(format[0]) >= 0)
            {
                return string.Format($"{{0:{formatStr}}}", Convert.ToInt64(value));
            }
            else if (NameSlotConstants.NumberFormatR.IndexOf(format[0]) >= 0)
            {
                return string.Format($"{{0:{formatStr}}}", BigInteger.Parse(value));
            }

            return string.Format($"{{0:{formatStr}}}", value);
        }

        private static string MatchValue(string value, string regex)
        {
            Match result = default;
            if (regex.EndsWith("/i"))
            {
                regex = regex[1..^2];
                result = Regex.Match(value, regex, RegexOptions.IgnoreCase);
            }
            else if (regex.EndsWith("/m"))
            {
                regex = regex[1..^2];
                result = Regex.Match(value, regex, RegexOptions.Multiline);
            }
            else if (regex.EndsWith("/s"))
            {
                regex = regex[1..^2];
                result = Regex.Match(value, regex, RegexOptions.Singleline);
            }
            else if (regex.EndsWith("/g"))
            {
                regex = regex[1..^2];
                result = Regex.Match(value, regex);
            }
            else 
            {
                regex = regex[1..^1];
                result = Regex.Match(value, regex);
            }

            return result.Success ? result.Value : string.Empty;
        }
    }
}
