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

        //TODO 支持简单计算
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

                    var ns = new NameSlot(p);
                    JsonElement leftProperty = default;
                    JsonElement rightProperty = default;

                    if (ns.LeftToken.TokenType == NameSlotTokenType.Property)
                    {
                        if (!element.TryGetPropertyWithJsonPath(ns.LeftToken.Token, out leftProperty))
                        {
                            throw new InvalidOperationException($"nameslot:{Encoding.UTF8.GetString(ns.LeftToken.Token)} not found");
                        }
                    }

                    if (ns.RightToken.TokenType == NameSlotTokenType.Property)
                    {
                        if (!element.TryGetPropertyWithJsonPath(ns.RightToken.Token, out rightProperty))
                        {
                            throw new InvalidOperationException($"nameslot:{Encoding.UTF8.GetString(ns.RightToken.Token)} not found");
                        }
                    }

                    if (ns.Operator != NameSlotOperator.None)
                    {
                        if (ns.LeftToken.TokenType == NameSlotTokenType.Property 
                            && ns.RightToken.TokenType == NameSlotTokenType.Property)
                        {
                            Caculate(sb, leftProperty.GetDecimal(), rightProperty.GetDecimal(), ns.Operator);
                        }
                        else if (ns.LeftToken.TokenType == NameSlotTokenType.Property
                            && ns.RightToken.TokenType == NameSlotTokenType.Number)
                        {
                            Caculate(sb, leftProperty.GetDecimal(), decimal.Parse(Encoding.UTF8.GetString(ns.RightToken.Token)), ns.Operator);
                        }
                        else if (ns.LeftToken.TokenType == NameSlotTokenType.Number
                            && ns.RightToken.TokenType == NameSlotTokenType.Property)
                        {
                            Caculate(sb, decimal.Parse(Encoding.UTF8.GetString(ns.LeftToken.Token)), rightProperty.GetDecimal(), ns.Operator);
                        }
                    }
                    else if (ns.LeftToken.TokenType == NameSlotTokenType.Property)
                    {
                        switch (leftProperty.ValueKind)
                        {
                            case JsonValueKind.String:
                                sb.Append(leftProperty.GetString());
                                break;
                            case JsonValueKind.Number:
                                sb.Append(leftProperty.GetDecimal());
                                break;
                            case JsonValueKind.True:
                            case JsonValueKind.False:
                                sb.Append(leftProperty.GetBoolean());
                                break;
                            case JsonValueKind.Null:
                                break;
                            default:
                                throw new InvalidOperationException($"nameslot:{Encoding.UTF8.GetString(p)} invaild value kind");
                        }
                    }

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

        private static void Caculate(StringBuilder sb, decimal leftValue, decimal rightValue, NameSlotOperator op)
        {
            switch (op)
            {
                case NameSlotOperator.Add:
                    sb.Append(leftValue + rightValue);
                    break;
                case NameSlotOperator.Minus:
                    sb.Append(leftValue - rightValue);
                    break;
                case NameSlotOperator.Multiply:
                    sb.Append(leftValue * rightValue);
                    break;
                case NameSlotOperator.Divide:
                    sb.Append(leftValue / rightValue);
                    break;
                default:
                    break;
            }

        }
    }
}
