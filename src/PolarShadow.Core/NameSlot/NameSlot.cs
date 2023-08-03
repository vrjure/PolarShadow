﻿using System;
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

        private static void Format(StringBuilder sb, ref NameSlotReader reader, IParameter parameter)
        {
            string currentValue = string.Empty;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case NameSlotTokenType.None:
                    case NameSlotTokenType.Start:
                        currentValue = string.Empty;
                        break;
                    case NameSlotTokenType.End:
                        sb.Append(currentValue);
                        break;
                    case NameSlotTokenType.Text:
                        sb.Append(reader.GetString());
                        break;
                    case NameSlotTokenType.Parameter:
                        currentValue = ReadParameter(ref reader, parameter);
                        break;
                    case NameSlotTokenType.Format:
                        currentValue = FormatValue(ref reader, currentValue);
                        break;
                    case NameSlotTokenType.Match:
                        currentValue = MatchValue(ref reader, currentValue);
                        break;
                    case NameSlotTokenType.Condition:
                    case NameSlotTokenType.ConditionOperator:
                        currentValue = CompareValue(ref reader, parameter, currentValue);
                        break;
                }
            }

        }

        private static string ReadParameter(ref NameSlotReader reader, IParameter parameter)
        {
            var result = reader.GetString();

            if (parameter.TryGetValue(result, out ParameterValue newResult))
            {
                return newResult.GetValue();
            }
            else
            {
                return string.Empty;
            }
        }

        private static string CompareValue(ref NameSlotReader reader, IParameter parameter, string currentValue)
        {
            if (reader.TokenType == NameSlotTokenType.Condition)
            {
                if (string.IsNullOrEmpty(currentValue))
                {
                    reader.Read();
                    reader.Read();
                    reader.Read();
                    return reader.GetString().Format(parameter);
                }
                else
                {
                    reader.Read();
                    return reader.GetString().Format(parameter);
                }
            }
            else if (reader.TokenType == NameSlotTokenType.ConditionOperator)
            {
                var op = reader.GetString();
                reader.Read();
                var compareValue = reader.GetString();
                reader.Read();
                reader.Read();
                var trueValue = reader.GetString();
                reader.Read();
                reader.Read();
                var falseValue = reader.GetString();

                return CompareValue(currentValue, op, compareValue, trueValue, falseValue);
            }
            return string.Empty;
        }


        private static string CompareValue(string left, string op, string right, string trueValue, string falseValue)
        {
            decimal leftDec = 0;
            decimal rightDec = 0;

            if (op == ">" || op =="<" || op ==">=" || op == "<=")
            {
                if (!decimal.TryParse(left, out leftDec)) return falseValue;
                if (!decimal.TryParse(right, out rightDec)) return falseValue;
            }

            return op switch
            {
                ">" => leftDec > rightDec ? trueValue : falseValue,
                "<" => leftDec < rightDec ? trueValue : falseValue,
                ">=" => leftDec >= rightDec ? trueValue : falseValue,
                "<=" => leftDec <= rightDec ? trueValue : falseValue,
                "==" => left == right ? trueValue : falseValue,
                "!=" => left != right ? trueValue : falseValue,
                _ => falseValue
            };
        }

        private static string FormatValue(ref NameSlotReader reader, string value)
        {
            var format = reader.GetSegment();

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

        private static string MatchValue(ref NameSlotReader reader, string value)
        {
            var regex = reader.GetString();

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
