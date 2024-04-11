using CommunityToolkit.Mvvm.ComponentModel;
using PolarShadows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PolarShadow.ToolBar
{
    sealed class ToolBarAttached
    {
        private static Dictionary<string, ContentControl> _containers = new Dictionary<string, ContentControl>(StringComparer.OrdinalIgnoreCase);

        
        public static readonly DependencyProperty NameProperty = DP.RegisterAttached<ContentControl,string>("Name", PropertyChanged);
        public static string GetName(ContentControl control)
        {
            return (string)control.GetValue(NameProperty);
        }
        public static void SetName(ContentControl control, string value)
        {
            control.SetValue(NameProperty, value);
        }

        public static readonly DependencyProperty ToolBarProperty = DP.RegisterAttached<ContentControl, List<ToolBarTemplate>>("ToolBar", PropertyChanged);
        public static List<ToolBarTemplate> GetToolBar(ContentControl control)
        {
            var templates = control.GetValue(ToolBarProperty);
            if (templates == null)
            {
                SetToolBar(control, new List<ToolBarTemplate>());
            }
            return (List<ToolBarTemplate>)control.GetValue(ToolBarProperty);
        }
        public static void SetToolBar(ContentControl control, List<ToolBarTemplate> value)
        {
            control.SetValue(ToolBarProperty, value);
        }

        public static void TryLoad(ContentControl page)
        {
            var templates = GetToolBar(page);
            foreach (var template in templates)
            {
                TryLoadToolBar(template, page);
            }
        }

        private static void TryLoadToolBar(ToolBarTemplate template, ContentControl page)
        {
            if (template == null || !template.HasContent) return;

            if (!_containers.TryGetValue(template.ToolBar, out ContentControl parent)) return;

            var result = template.LoadContent() as FrameworkElement;
            if (result == null) return;

            result.DataContext = page.DataContext;
            parent.Content = result;
        }

        public static void TryUnload(ContentControl page)
        {
            var templates = GetToolBar(page);
            foreach (var template in templates)
            {
                TryUnloadToolBar(template);
            }
        }

        private static void TryUnloadToolBar(ToolBarTemplate template)
        {
            if (template == null) return;
            if (!_containers.TryGetValue(template.ToolBar, out ContentControl parent)) return;
            if (parent.Content is Control child) child.DataContext = null;

            parent.Content = null;
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == NameProperty)
            {
                NamePropertyChanged(d, e);
            }
            else if (e.Property == ToolBarProperty)
            {
                ToolBarPropertyChanged(d, e);
            }
        }

        private static void NamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ContentControl) return;
            var value = e.NewValue as string;
            if (!_containers.TryGetValue(value, out ContentControl container))
            {
                container = d as ContentControl;
                _containers.Add(value, container);
                container.AddHandler(ContentControl.UnloadedEvent, new RoutedEventHandler(Container_Unloaded));
            }

            static void Container_Unloaded(object sender, RoutedEventArgs e)
            {
                var name = GetName(sender as ContentControl);
                if (string.IsNullOrEmpty(name)) return;
                _containers.Remove(name);
            }
        }

        private static void ToolBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = d as ContentControl;
            page.AddHandler(ContentControl.UnloadedEvent, new RoutedEventHandler(ToolBarTemplate_Unloaded));
            page.AddHandler(ContentControl.LoadedEvent, new RoutedEventHandler(toolBarTemplate_Loaded));

            static void toolBarTemplate_Loaded(object sender, RoutedEventArgs e)
            {
                var control = sender as ContentControl;

                TryLoad(control);
            }

            static void ToolBarTemplate_Unloaded(object sender, RoutedEventArgs e)
            {
                var control = sender as ContentControl;

                TryUnload(control);
            }
        }
    }
}
