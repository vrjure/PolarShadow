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
        public static DependencyProperty Register<TOwner, TValue>(string name, PropertyChangedCallback changed)
        {
            return Register<TOwner, TValue>(name, default, changed);
        }

        public static DependencyProperty Register<TOwner, TValue>(string name, TValue defaultValue = default, PropertyChangedCallback changed = default, CoerceValueCallback coerceValueCallback = default, ValidateValueCallback validateValueCallback = default, Func<PropertyMetadata> metadataCreator = default)
        {
            var metadata = metadataCreator?.Invoke() ?? new PropertyMetadata();
            metadata.DefaultValue = defaultValue;
            metadata.PropertyChangedCallback = changed;
            metadata.CoerceValueCallback = coerceValueCallback;

            return validateValueCallback == default ?
                DependencyProperty.Register(name, typeof(TValue), typeof(TOwner), metadata)
                : DependencyProperty.Register(name, typeof(TValue), typeof(TOwner), metadata, validateValueCallback);
        }

        public static DependencyProperty RegisterAttached<TOwner, TValue>(string name, PropertyChangedCallback changed)
        {
            return RegisterAttached<TOwner, TValue>(name, default, changed);
        }

        public static DependencyProperty RegisterAttached<TOwner, TValue>(string name, TValue defaultValue = default, PropertyChangedCallback changed = default, CoerceValueCallback coerceValueCallback = default, ValidateValueCallback validateValueCallback = default, Func<PropertyMetadata> metadataCreator = default)
        {
            var metadata = metadataCreator?.Invoke() ?? new PropertyMetadata();
            metadata.DefaultValue = defaultValue;
            metadata.PropertyChangedCallback = changed;
            metadata.CoerceValueCallback = coerceValueCallback;

            return validateValueCallback == default ? 
                DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(TOwner), metadata) 
                : DependencyProperty.RegisterAttached(name, typeof(TValue), typeof(TOwner), metadata, validateValueCallback);
        }
    }
}
