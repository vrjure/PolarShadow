using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal static class StateContextExtensions
    {
        public static bool TryGet<T>(this IStateContext context, string key, out T value)
        {
            if(context.TryGet(key, out object val) && val is T v)
            {
                value = v;
                return true;
            }

            value = default;
            return false;
        }
    }
}
