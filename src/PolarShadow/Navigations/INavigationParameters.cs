using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public interface INavigationParameters : IEnumerable<KeyValuePair<string, object>>
    {
        int Count { get; }
        void Add(string key, object value);
        bool ContainKey(string key);
        bool TryGetValue(string key, out object value);
        IEnumerable<string> Keys { get; }
        IEnumerable<object> Values { get; }
    }
}
