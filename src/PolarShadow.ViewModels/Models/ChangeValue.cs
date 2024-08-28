using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Models
{
    public class ChangeValue<T> : ObservableObject
    {
        public ChangeValue(T compare) : this(compare, compare)
        {

        }
        public ChangeValue(T compare, T value)
        {
            this.Compare = compare;
            this.Value = value;
        }
        public T Compare { get; private set; }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
            }
        }

        public bool IsChange => !EqualityComparer<T>.Default.Equals(Compare, Value);

        public void Reset()
        {
            Compare = Value;
        }
        public void Reset(T value)
        {
            Compare = Value = value;
        }
    }
}
