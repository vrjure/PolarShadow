using PolarShadow.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal class KeyNameCollection<T> : IKeyNameCollection<T> where T : IKeyName
    {
        private readonly Dictionary<string, T> _keyValues = new Dictionary<string, T>();
        public T this[string key] => _keyValues[key];

        public int Count => _keyValues.Count;

        public bool IsReadOnly => false;

        public IEnumerable<string> Keys => _keyValues.Keys;

        public IEnumerable<T> Values => _keyValues.Values;

        public KeyNameCollection() { }
        public KeyNameCollection(IEnumerable<T> values) 
        {
            foreach (var item in values)
            {
                _keyValues.Add(item.Name, item);
            }
        }

        public void Add(T item)
        {
            _keyValues[item.Name] = item;
        }

        public void Clear()
        {
            _keyValues.Clear();
        }

        public bool Contains(T item)
        {
            return _keyValues.ContainsKey(item.Name);
        }

        public bool ContainsKey(string key)
        {
            return _keyValues.ContainsKey(key);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _keyValues.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _keyValues.Values.GetEnumerator();
        }

        public bool Remove(T item)
        {
            return _keyValues.Remove(item.Name);
        }

        public bool RemoveKey(string key)
        {
            return _keyValues.Remove(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            return _keyValues.TryGetValue(key, out value);
        }

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            return _keyValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _keyValues.GetEnumerator();
        }
    }
}
