using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Messaging;
using PolarShadow.Controls;
using PolarShadow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public sealed class NavigationManager
    {
        private static Dictionary<string, ContentControl> _containers = new Dictionary<string, ContentControl>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, Control> _backButtons = new Dictionary<string, Control>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, Stack<Control>> _history = new Dictionary<string, Stack<Control>>();

        static NavigationManager()
        {
            ContainerNameProperty.Changed.Subscribe(ContainerNamePropertyChanged);
            BackNameProperty.Changed.Subscribe(BackButtonNamePropertyChanged);
        }

        public static bool TryGetContainer(string containerName, out ContentControl container)
        {
            return _containers.TryGetValue(containerName, out container);
        }

        public static bool TryGetBackButton(string backName, out Control input)
        {
            return _backButtons.TryGetValue(backName, out input);
        }

        public static bool CanBack(string container, out Stack<Control> his)
        {
            if (!_history.TryGetValue(container, out his)) return false;
            if (his.Count <= 0) return false;

            return true;
        }

        public static void Back(string container)
        {
            if (!CanBack(container, out Stack<Control> his)) return;

            var view = his.Pop();

            Navigate(container, view, default, false);
        }

        public static void Navigate(string containerName, Control page, IDictionary<string, object> parameters, bool canBack)
        {
            if (!TryGetContainer(containerName, out ContentControl container))
            {
                throw new InvalidOperationException($"Container name [{containerName}] not found");
            }

            var current = container.Content as Control;

            if (current != null)
            {
                if (canBack)
                {
                    if (!_history.TryGetValue(containerName, out Stack<Control> his))
                    {
                        his = new Stack<Control>();
                        _history[containerName] = his;
                    }
                    his.Push(current);
                }
            }

            if (parameters != null && page.DataContext is IParameterObtain po)
            {
                po.ApplyParameter(parameters);
            }

            container.Content = page;

            if (TryGetBackButton(containerName, out Control btn))
            {
                btn.Opacity = CanBack(containerName, out Stack<Control> _) ? 1 : 0;
            }

        }

        public static readonly AttachedProperty<string> ContainerNameProperty = AvaloniaProperty.RegisterAttached<NavigationManager, ContentControl, string>("ContainerName");
        public static string GetContainerName(ContentControl control)
        {
            return control.GetValue(ContainerNameProperty);
        }
        public static void SetContainerName(ContentControl control, string value)
        {
            control.SetValue(ContainerNameProperty, value);
        }

        public static readonly AttachedProperty<string> BackNameProperty = AvaloniaProperty.RegisterAttached<NavigationManager, Control, string>("BackName");
        public static string GetBackName(Control control)
        {
            return control.GetValue(BackNameProperty);
        }
        public static void SetBackName(Control control, string value)
        {
            control.SetValue(BackNameProperty, value);
        }

        private static void ContainerNamePropertyChanged(AvaloniaPropertyChangedEventArgs<string> args)
        {
            if (Design.IsDesignMode || !args.NewValue.HasValue) return;
            var control = args.Sender as ContentControl;
            var containerName = args.NewValue.Value;
            if (!_containers.TryGetValue(containerName, out ContentControl container))
            {
                container = control;
                container.AddHandler(ContentControl.LoadedEvent, Container_Loaded);
                container.AddHandler(ContentControl.UnloadedEvent, Container_Unloaded);

                _containers.Add(containerName, container);
            }

            static void Container_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                var control = sender as ContentControl;
                var containerName = GetContainerName(control);
                if (!_containers.ContainsKey(containerName))
                {
                    _containers.Add(containerName, control);
                }
            }

            static void Container_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                var container = sender as ContentControl;

                var containerName = GetContainerName(container);

                if (string.IsNullOrEmpty(containerName)) return;
                _containers.Remove(containerName);
            }
        }

        private static void BackButtonNamePropertyChanged(AvaloniaPropertyChangedEventArgs<string> arg)
        {
            if (Design.IsDesignMode || !arg.NewValue.HasValue) return;
            if (!_backButtons.TryGetValue(arg.NewValue.Value, out Control backBtn))
            {
                backBtn = arg.Sender as Control;
                backBtn.AddHandler(Control.UnloadedEvent, backBtn_Unloaded);
                if (backBtn is Button btn)
                {
                    btn.AddHandler(Button.ClickEvent, Btn_Click);
                }
                else
                {
                    backBtn.AddHandler(Button.PointerReleasedEvent, BackBtn_PointerReleased);
                }
                backBtn.Opacity = 0;
                _backButtons.Add(arg.NewValue.Value, backBtn);
            }

            static void BackButtonClick(object sender)
            {
                var backName = GetBackName(sender as Control);
                if (string.IsNullOrEmpty(backName)) return;

                Back(backName);
            }

            static void Btn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                BackButtonClick(sender);
            }

            static void BackBtn_PointerReleased(object sender, PointerReleasedEventArgs e)
            {
                BackButtonClick(sender);
            }

            static void backBtn_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                var backName = GetBackName(sender as Control);
                if (string.IsNullOrEmpty(backName)) return;
                _backButtons.Remove(backName);
            }
        }
    }
}
