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
            NameProperty.Changed.AddClassHandler<ContentControl>((o, e)=>NamePropertyChanged(e));
            ToolBarProperty.Changed.AddClassHandler<ContentControl>((o,e)=>ToolBarPropertyChanged(e));
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

        public static readonly AttachedProperty<ICollection<IControlTemplate>> ToolBarProperty = AvaloniaProperty.RegisterAttached<ToolBarAttached, ContentControl, ICollection<IControlTemplate>>("ToolBar");
        public static ICollection<IControlTemplate> GetToolBar(ContentControl page)
        {
            var templates = page.GetValue(ToolBarProperty);
            if (templates == null)
            {
                SetToolBar(page, new List<IControlTemplate>());
            }

            return page.GetValue(ToolBarProperty);
        }
        public static void SetToolBar(ContentControl page, ICollection<IControlTemplate> value)
        {
            page.SetValue(ToolBarProperty, value);
        }

        public static void TryLoad(ContentControl page)
        {
            var templates = GetToolBar(page);
            foreach (var template in templates)
            {
                TryLoadToolBar(template as ToolBarTemplate, page);
            }
        }

        private static void TryLoadToolBar(ToolBarTemplate template, ContentControl page)
        {
            if (template == null) return;

            if (!_containers.TryGetValue(template.ToolBar, out ContentControl parent)) return;

            var result = template.Build(parent);
            if (result == null || result.Result == null) return;

            result.Result.DataContext = page.DataContext;
            parent.Content = result.Result;
        }

        public static void TryUnload(ContentControl page)
        {
            var templates = GetToolBar(page);
            foreach (var template in templates)
            {
                TryUnloadToolBar(template as ToolBarTemplate);
            }
        }

        private static void TryUnloadToolBar(ToolBarTemplate template)
        {
            if (template == null) return;
            if (!_containers.TryGetValue(template.ToolBar, out ContentControl parent)) return;
            if (parent.Content is Control child) child.DataContext = null;

            parent.Content = null;
        }

        private static void NamePropertyChanged(AvaloniaPropertyChangedEventArgs arg)
        {
            if (Design.IsDesignMode || arg.NewValue is not string) return;

            var value = arg.NewValue as string;
            if (!_containers.TryGetValue(value, out ContentControl container))
            {
                container = arg.Sender as ContentControl;
                _containers.Add(value, container);
                container.AddHandler(ContentControl.UnloadedEvent, Container_Unloaded);
            }

            static void Container_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                var name = (sender as ContentControl).GetValue(NameProperty);
                if (string.IsNullOrEmpty(name)) return;
                _containers.Remove(name);
            }
        }

        private static void ToolBarPropertyChanged(AvaloniaPropertyChangedEventArgs arg)
        {
            if (Design.IsDesignMode || arg.NewValue == null) return;

            var page = arg.Sender as ContentControl;
            page.AddHandler(ContentControl.UnloadedEvent, ToolBarTemplate_Unloaded);
            page.AddHandler(ContentControl.LoadedEvent, toolBarTemplate_Loaded);

            static void toolBarTemplate_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                var control = sender as ContentControl;

                TryLoad(control);
            }

            static void ToolBarTemplate_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                var control = sender as ContentControl;

                TryUnload(control);
            }
        }
    }
}
