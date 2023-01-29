using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Site
{
    public class SiteOptionCollection : ICollection<SiteOption>
    {
        private List<SiteOption> _configs = new List<SiteOption>();
        private bool _isChanged = false;
        public int Count => _configs.Count;

        public bool IsReadOnly => false;
        public bool IsChanged
        {
            get => _isChanged;
            set => _isChanged = value;
        }

        internal SiteOptionCollection()
        {

        }

        public void Add(SiteOption item)
        {
            _configs.Add(item);
            _isChanged = true;
        }

        public void Clear()
        {
            _configs.Clear();
            _isChanged = true;
        }

        public bool Contains(SiteOption item)
        {
            return _configs.Contains(item);
        }

        public void CopyTo(SiteOption[] array, int arrayIndex)
        {
            _configs.CopyTo(array, arrayIndex);
        }

        public IEnumerator<SiteOption> GetEnumerator()
        {
            return _configs.GetEnumerator();
        }

        public bool Remove(SiteOption item)
        {
            if(_configs.Remove(item))
            {
                _isChanged = true;
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
