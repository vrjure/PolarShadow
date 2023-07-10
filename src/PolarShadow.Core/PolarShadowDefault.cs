using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class PolarShadowDefault : IPolarShadow
    {
        private readonly Dictionary<string, IPolarShadowItem> _items = new Dictionary<string, IPolarShadowItem>();

        internal PolarShadowDefault(IEnumerable<IPolarShadowItem> items)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }
                _items.Add(item.Name, item);
            }
        }

        public IPolarShadowItem this[string name] => _items.ContainsKey(name) ? _items[name] : null;

        public IEnumerable<IPolarShadowItem> Items => _items.Values;
    }
}
