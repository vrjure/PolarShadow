using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public class NavigationParameters : INavigationParameters
    {
        List<KeyValuePair<string, object>> _source = new List<KeyValuePair<string, object>>();

        public int Count => _source.Count;

        public IEnumerable<string> Keys => _source.Select(f=>f.Key);

        public IEnumerable<object> Values => _source.Select(f => f.Value);

        public void Add(string key, object value)
        {
            if(ContainKey(key)) throw new InvalidOperationException($"Key [{key}] existed");
            _source.Add(new KeyValuePair<string, object>(key, value));
        }

        public bool ContainKey(string key)
        {
            return TryGetValue(key, out object _);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        public bool TryGetValue(string key, out object value)
        {
            value = default;
            if(string.IsNullOrEmpty(key)) return false;

            foreach (var item in _source)
            {
                if (string.Compare(item.Key, key, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    value = item.Value;
                    return true;
                }
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
