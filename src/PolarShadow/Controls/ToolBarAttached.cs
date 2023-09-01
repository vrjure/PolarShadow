using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public class ToolBarAttached
    {
        private static Dictionary<string, ContentControl> _containers = new Dictionary<string, ContentControl>(StringComparer.OrdinalIgnoreCase);
        static ToolBarAttached()
        {
            NameProperty.Changed.Subscribe(NamePropertyChanged);
            ToolBarProperty.Changed.Subscribe(ToolBarPropertyChanged);
        }

        public static readonly AttachedProperty<string> NameProperty = AvaloniaProperty.RegisterAttached<ToolBarAttached, ContentControl, string>("Name");
        public static string GetName(ContentControl content)
        {
            return content.GetValue(NameProperty);
        }
        public static void SetName(ContentControl content, string value)
        {
            content.SetValue(NameProperty, value);
        }

        public static readonly AttachedProperty<IControlTemplate> ToolBarProperty = AvaloniaProperty.RegisterAttached<ToolBarAttached, ContentControl, IControlTemplate>("ToolBar");
        public static IControlTemplate GetToolBar(ContentControl page)
        {
            return page.GetValue(ToolBarProperty);
        }
        public static void SetToolBar(ContentControl page, IControlTemplate value)
        {
            page.SetValue(ToolBarProperty, value);
        }

        public static void TryLoad(ContentControl page)
        {
            var toolBarTemplate = GetToolBar(page);

            if (toolBarTemplate is not ToolBarTemplate template) return;
            page.Unloaded += ToolBarTemplate_Unloaded;

            if (!_containers.TryGetValue(template.ToolBar, out ContentControl parent)) return;

            var result = template.Build(parent);
            if (result == null || result.Result == null) return;

            result.Result.DataContext = page.DataContext;
            parent.Content = result.Result;
        }

        private static void NamePropertyChanged(AvaloniaPropertyChangedEventArgs<string> arg)
        {
            if (Design.IsDesignMode || !arg.NewValue.HasValue) return;

            if (!_containers.TryGetValue(arg.NewValue.Value, out ContentControl container))
            {
                container = arg.Sender as ContentControl;
                _containers.Add(arg.NewValue.Value, container);
                container.Unloaded += Container_Unloaded;
            }
        }

        private static void ToolBarPropertyChanged(AvaloniaPropertyChangedEventArgs<IControlTemplate> arg)
        {
            if (Design.IsDesignMode || !arg.NewValue.HasValue) return;
            if (!arg.NewValue.HasValue || arg.NewValue.Value is not ToolBarTemplate template) return;

            if (!_containers.TryGetValue(template.ToolBar, out ContentControl container)) return;

            var toolBarTemplate = arg.Sender as ContentControl;
            toolBarTemplate.Loaded += toolBarTemplate_Loaded;
            toolBarTemplate.Unloaded += ToolBarTemplate_Unloaded;
        }

        private static void toolBarTemplate_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var control = sender as ContentControl;
            control.Loaded -= toolBarTemplate_Loaded;

            TryLoad(control);
        }

        private static void ToolBarTemplate_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var control = sender as ContentControl;
            control.Unloaded -= ToolBarTemplate_Unloaded;

            if (GetToolBar(control) is not ToolBarTemplate template) return;
            if (!_containers.TryGetValue(template.ToolBar, out ContentControl parent)) return;
            if (parent.Content is Control child) child.DataContext = null;
            
            parent.Content = null;       
        }

        private static void Container_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            (sender as ContentControl).Unloaded -= Container_Unloaded;

            var name = (sender as ContentControl).GetValue(NameProperty);
            if (string.IsNullOrEmpty(name)) return;
            _containers.Remove(name);
        }
    }
}
