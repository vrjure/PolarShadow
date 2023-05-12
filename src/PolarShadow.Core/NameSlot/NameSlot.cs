using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PolarShadow.Core
{
    public static class NameSlot
    {
        public static string Format(this string text, NameSlotValueCollection values)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return Format(Encoding.UTF8.GetBytes(text), values);
        }
        public static string Format(this ReadOnlySpan<byte> text, NameSlotValueCollection values)
        {
            if (text.IsEmpty)
            {
                return string.Empty;
            }
            var sb = new StringBuilder(text.Length);
            var reader = new NameSlotReader(text);
            Format(sb, ref reader, values);
            return sb.ToString();
        }

        private static void Format(StringBuilder sb, ref NameSlotReader reader, NameSlotValueCollection values)
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
                        ReadValue(sb, ref reader, values);
                        break;
                }
            }

        }

        private static void ReadValue(StringBuilder sb, ref NameSlotReader reader, NameSlotValueCollection values)
        {
            var result = string.Empty;
            if (reader.TokenType == NameSlotTokenType.Parameter)
            {
                result = reader.GetString();
            }

            reader.Read();
            if (values.TryReadValue(result, out NameSlotValue newResult))
            {
                result = newResult.GetValue();

                if (reader.TokenType == NameSlotTokenType.Format)
                {
                    var format = reader.GetString();
                    result = FormatValue(result, format);
                }
                else if (reader.TokenType == NameSlotTokenType.Match)
                {
                    var regex = reader.GetString();
                    result = MatchValue(result, regex);
                }
            }
            else
            {
                result = string.Empty;
            }
            
            sb.Append(result);
        }

        private static string FormatValue(string value, string format)
        {
            if (NameSlotConstants.NumberFormatCommonChars.IndexOf((byte)format[0]) > 0)
            {
                return string.Format($"{{0:{format}}}", Convert.ToDecimal(value));
            }
            else if (NameSlotConstants.NumberFormatR.IndexOf((byte)format[0]) > 0)
            {
                return string.Format($"{{0:{format}}}", Convert.ToInt64(value));
            }

            return string.Format($"{{0:{format}}}", value);
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
