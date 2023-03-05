using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal class StateContext : IStateContext
    {
        private static readonly Dictionary<string, object> _context = new Dictionary<string, object>();

        public void Remove(string key)
        {
            _context.Remove(key);
        }

        public void Set(string key, object value)
        {
            _context[key] = value;
        }

        public bool TryGet(string key, out object value)
        {
            return _context.TryGetValue(key, out value);
        }
    }
}
