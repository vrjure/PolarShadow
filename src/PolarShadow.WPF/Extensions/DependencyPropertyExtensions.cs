using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolarShadows
{
    static class DP
    {
        public static DependencyProperty Register<TOwner, TValue>(string name)
        {
            return DependencyProperty.Register(name, typeof(TValue), typeof(TOwner));
        }

        public static DependencyProperty Register<TOwner, TValue>(string name, TValue defaultValue)
        {
            return DependencyProperty.Register(name, typeof(TValue), typeof(TOwner), new PropertyMetadata(defaultValue));
        }

        public static DependencyProperty Register<TOwner, TValue>(string name, TValue defaultValue, PropertyChangedCallback changed)
        {
            return DependencyProperty.Register(name, typeof(TValue), typeof(TOwner), new PropertyMetadata(defaultValue, changed));
        }

        public static DependencyProperty Register<TOwner, TValue>(string name, TValue defaultValue, PropertyChangedCallback changed, CoerceValueCallback coerceValueCallback)
        {
            return DependencyProperty.Register(name, typeof(TValue), typeof(TOwner), new PropertyMetadata(defaultValue, changed, coerceValueCallback));
        }

        public static DependencyProperty Register<TOwner, TValue>(string name, TValue defaultValue, PropertyChangedCallback changed, CoerceValueCallback coerceValueCallback, ValidateValueCallback validateValueCallback)
        {
            return DependencyProperty.Register(name, typeof(TValue), typeof(TOwner), new PropertyMetadata(defaultValue, changed, coerceValueCallback), validateValueCallback);
        }



        public static DependencyProperty RegisterAttached<TOwner, TValue>(string name)
        {
            return DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(TOwner));
        }

        public static DependencyProperty RegisterAttached<TOwner, TValue>(string name, TValue defaultValue)
        {
            return DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(TOwner), new PropertyMetadata(defaultValue));
        }

        public static DependencyProperty RegisterAttached<TOwner, TValue>(string name, PropertyChangedCallback changed)
        {
            return DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(TOwner), new PropertyMetadata(changed));
        }

        public static DependencyProperty RegisterAttached<TOwner, TValue>(string name, TValue defaultValue, PropertyChangedCallback changed)
        {
            return DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(TOwner), new PropertyMetadata(defaultValue, changed));
        }

        public static DependencyProperty RegisterAttached<TOwner, TValue>(string name, TValue defaultValue, PropertyChangedCallback changed, CoerceValueCallback coerceValueCallback)
        {
            return DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(TOwner), new PropertyMetadata(defaultValue, changed, coerceValueCallback));
        }

        public static DependencyProperty RegisterAttached<TOwner, TValue>(string name, TValue defaultValue, PropertyChangedCallback changed, CoerceValueCallback coerceValueCallback, ValidateValueCallback validateValueCallback)
        {
            return DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(TOwner), new PropertyMetadata(defaultValue, changed, coerceValueCallback), validateValueCallback);
        }
    }
}
