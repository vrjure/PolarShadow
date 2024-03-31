using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PolarShadow.Navigations
{
    internal sealed partial class NavigationManager
    {

        public static readonly DependencyProperty ContainerNameProperty = DependencyProperty.RegisterAttached("ContainerName", typeof(string), typeof(ContentControl), new PropertyMetadata("", PropertyChanged));
        public static string GetContainerName(ContentControl control)
        {
            return (string)control.GetValue(ContainerNameProperty);
        }
        public static void SetContainerName(ContentControl control, string value)
        {
            control.SetValue(ContainerNameProperty, value);
        }

        public static readonly DependencyProperty BackNameProperty = DependencyProperty.RegisterAttached("BackName", typeof(string), typeof(FrameworkElement), new PropertyMetadata("", PropertyChanged));
        public static string GetBackName(FrameworkElement control)
        {
            return (string)control.GetValue(BackNameProperty);
        }
        public static void SetBackName(FrameworkElement control, string value)
        {
            control.SetValue(BackNameProperty, value);
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ContainerNameProperty)
            {
                ContainerNameChanged(d as ContentControl, e);
            }
            else if (e.Property == BackNameProperty)
            {
                BackNameChagned(d as FrameworkElement, e);
            }
        }

        private static void ContainerNameChanged(ContentControl container, DependencyPropertyChangedEventArgs e)
        {
            var containerName = e.NewValue as string;
            if (_containers.ContainsKey(containerName))
            {
                return;
            }
            else
            {
                container.AddHandler(ContentControl.LoadedEvent, new RoutedEventHandler(ContainerLoaded));
                container.AddHandler(ContentControl.UnloadedEvent, new RoutedEventHandler(ContainerUnloaded));
                _containers.Add(containerName, container);
            }

            static void ContainerLoaded(object sender, RoutedEventArgs e)
            {
                var container = sender as ContentControl;
                var containerName = GetContainerName(container);
                if (!_containers.ContainsKey(containerName))
                {
                    _containers.Add(containerName, container);
                }
            }

            static void ContainerUnloaded(object sender, RoutedEventArgs e)
            {
                var container = sender as ContentControl;
                var containerName = GetContainerName(container);
                if (string.IsNullOrEmpty(containerName)) return;
                _containers.Remove(containerName);
            }
        }

        private static void BackNameChagned(FrameworkElement control, DependencyPropertyChangedEventArgs e)
        {
            if (control == null)
            {
                return;
            }
            var backName = e.NewValue as string;
            if (!_backButtons.ContainsKey(backName))
            {
                control.AddHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(BackButtonUnloaded));
                if (control is Button btn)
                {
                    btn.AddHandler(Button.ClickEvent, new RoutedEventHandler(BackButtonClick));
                }
                else
                {
                    control.AddHandler(FrameworkElement.MouseUpEvent, new RoutedEventHandler(BackButtonPointReleased));
                }
                _backButtons.Add(backName, control);
            }

            static void BackButtonUnloaded(object sender, RoutedEventArgs e)
            {
                var backBtn = sender as FrameworkElement;
                var backBtnName = GetBackName(backBtn);
                if (string.IsNullOrEmpty(backBtnName))
                {
                    return;
                }
                _backButtons.Remove(backBtnName);
            }

            static void BackClick(object sender)
            {
                var btnName = GetBackName(sender as FrameworkElement);
                if (string.IsNullOrEmpty(btnName))
                {
                    return;
                }

                Back(btnName);
            }

            static void BackButtonClick(object sender, RoutedEventArgs e)
            {
                BackClick(sender);
            }

            static void BackButtonPointReleased(object sender, RoutedEventArgs e)
            {
                BackClick(sender);
            }
        }
    }
}
