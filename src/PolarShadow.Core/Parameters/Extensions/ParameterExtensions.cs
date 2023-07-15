﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public static partial class ParameterExtensions
    {
        public static bool TryReadValue<TValue>(this IParameter parameter, string path, out TValue value)
        {
            if (parameter.TryGetValue(path, out ParameterValue val))
            {
                switch (val.ValueKind)
                {
                    case ParameterValueKind.Number:
                        if (val.GetDecimal() is TValue vd) { value = vd; return true; }
                        else if (val.GetInt16() is TValue v16) { value = v16; return true; }
                        else if (val.GetInt32() is TValue v32) { value = v32; return true; }
                        else if (val.GetInt64() is TValue v64) { value = v64; return true; }
                        else if (val.GetFloat() is TValue vfloat) { value = vfloat; return true; }
                        else if (val.GetDouble() is TValue vdouble) { value = vdouble; return true; }
                        break;
                    case ParameterValueKind.String:
                        if (val.GetString() is TValue v)
                        {
                            value = v;
                            return true;
                        }
                        break;
                    case ParameterValueKind.Boolean:
                        if (val.GetBoolean() is TValue vb)
                        {
                            value = vb;
                            return true;
                        }
                        break;
                    case ParameterValueKind.Json:
                        if (val.GetJson() is TValue vj)
                        {
                            value = vj;
                            return true;
                        }
                        break;
                    case ParameterValueKind.Html:
                        if (val.GetHtml() is TValue vh)
                        {
                            value = vh;
                            return true;
                        }
                        break;
                    default:
                        break;
                }
            }

            value = default;
            return false;
        }
    }
}