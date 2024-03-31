using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    public static class DictionaryExtensions
    {
        public static bool TryGetValue<T>(this IDictionary<string, object> dict, string key, out T value)
        {
            value = default(T);
            if (!dict.TryGetValue(key, out object temp)) return false;

            if (temp is T convert)
            {
                value = convert;
                return true;
            }
            return false;
        }
    }
}
