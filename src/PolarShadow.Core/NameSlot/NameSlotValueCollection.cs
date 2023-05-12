using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class NameSlotValueCollection : ICollection<NameSlotValue>
    {
        private readonly List<NameSlotValue> _source = new List<NameSlotValue>();
        public int Count => _source.Count;

        public bool IsReadOnly => false;

        public void Add(NameSlotValue item)
        {
            _source.Add(item);
        }

        public void Clear()
        {
            _source.Clear();
        }

        public bool Contains(NameSlotValue item)
        {
            return _source.Contains(item);
        }

        public void CopyTo(NameSlotValue[] array, int arrayIndex)
        {
            _source.CopyTo(array, arrayIndex);
        }

        public IEnumerator<NameSlotValue> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        public bool Remove(NameSlotValue item)
        {
            return _source.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        public bool TryReadValue(string path, out NameSlotValue value)
        {
            value = default;
            foreach (var item in _source)
            {
                value = item.ReadValue(path);
                if (value.ValueKind == NameSlotValueKind.Undefined)
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
