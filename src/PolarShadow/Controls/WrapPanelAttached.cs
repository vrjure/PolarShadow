using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public sealed class WrapPanelAttached
    {
        static WrapPanelAttached()
        {
            SourceProperty.Changed.Subscribe(SourcePropertyChanged);
            ItemTemplateProperty.Changed.Subscribe(ItemTemplatePropertyChanged);
        }

        public static readonly AttachedProperty<IEnumerable> SourceProperty = AvaloniaProperty.RegisterAttached<WrapPanelAttached, WrapPanel, IEnumerable>("Source");
        public static IEnumerable GetSource(WrapPanel panel)
        {
            return panel.GetValue(SourceProperty);
        }
        public static void SetSource(WrapPanel panel, IEnumerable value)
        {
            panel.SetValue(SourceProperty, value);
        }

        public static readonly AttachedProperty<IDataTemplate> ItemTemplateProperty = AvaloniaProperty.RegisterAttached<WrapPanelAttached, WrapPanel, IDataTemplate>("ItemTemplate");
        public static IDataTemplate GetItemTemplate(WrapPanel panel)
        {
            return panel.GetValue(ItemTemplateProperty);
        }
        public static void SetItemTemplate(WrapPanel panel, IDataTemplate value)
        {
            panel.SetValue(ItemTemplateProperty, value);
        }

        private static void SourcePropertyChanged(AvaloniaPropertyChangedEventArgs<IEnumerable> args)
        {
            if (args.Sender is not WrapPanel panel) return;

            DataChanged(panel, args.NewValue.Value, default);
        }

        private static void ItemTemplatePropertyChanged(AvaloniaPropertyChangedEventArgs<IDataTemplate> args)
        {
            if (args.Sender is not WrapPanel panel) return;

            DataChanged(panel, default, args.NewValue.Value);
        }

        private static void DataChanged(WrapPanel panel, IEnumerable newSource, IDataTemplate newItemTemplate)
        {
            if (newSource == null)
                newSource = GetSource(panel);
            if (newItemTemplate == null)
                newItemTemplate = GetItemTemplate(panel);

            if (newSource == null || newItemTemplate == null) return;
            
            var o = newSource.GetEnumerator();
            while (o.MoveNext())
            {
                if (!newItemTemplate.Match(o.Current)) return;
                panel.Children.Add(newItemTemplate.Build(o.Current));
            }
        }
    }
}
