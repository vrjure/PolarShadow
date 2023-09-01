using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using PolarShadow.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    internal sealed class NavigationManager
    {
        private static Dictionary<string, ContentControl> _containers = new Dictionary<string, ContentControl>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, Control> _backButtons = new Dictionary<string, Control>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, Stack<Control>> _history = new Dictionary<string, Stack<Control>>();
        private static HashSet<string> _hasContainers = new HashSet<string>();
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

                    var hasContainer = GetHasContainer(current);
                    if (!string.IsNullOrEmpty(hasContainer))
                    {
                        _hasContainers.Add(hasContainer);
                    }
                }

                current.Unloaded += Page_Unloaded;
            }

            page.Loaded += Page_Loaded;

            container.Content = page;

            if (parameters != null && page.DataContext is IParameterObtain po)
            {
                po.ApplyParameter(parameters);
            }

            if (TryGetBackButton(containerName, out Control btn))
            {
                btn.IsVisible = CanBack(containerName, out Stack<Control> _);
            }

        }

        private static void Page_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var control = sender as Control;
            control.Unloaded -= Page_Unloaded;
            if (control.DataContext is INavigationNotify newNotify)
            {
                newNotify.OnUnload();
            }
        }

        private static void Page_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var control = sender as Control;
            control.Loaded -= Page_Loaded;

            var hasContainer = GetHasContainer(control);
            if (control is ContentControl cc && !string.IsNullOrEmpty(hasContainer))
            {
                TryLoadToolBar(cc);
            }

            if (!string.IsNullOrEmpty(hasContainer))
            {
                _hasContainers.Remove(hasContainer);
            }

            if (control.DataContext is INavigationNotify newNotify)
            {
                newNotify.OnLoad();
            }
        }

        private static void TryLoadToolBar(ContentControl contentControl)
        {
            ToolBarAttached.TryLoad(contentControl);

            var hasContainer = GetHasContainer(contentControl);
            if (string.IsNullOrEmpty(hasContainer) || !TryGetContainer(hasContainer,out ContentControl childContiner)) return;
            if (childContiner.Content is not ContentControl chidlContainerContent) return;
            TryLoadToolBar(chidlContainerContent);            
        }

        public static readonly AttachedProperty<string> HasContainerProperty = AvaloniaProperty.RegisterAttached<NavigationManager, Control, string>("HasContainer");
        public static string GetHasContainer(Control control)
        {
            return control.GetValue(HasContainerProperty);
        }

        public static void SetHasContainer(Control control, string value)
        {
            control.SetValue(HasContainerProperty, value);
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
                container.Loaded += Container_Loaded;
                container.Unloaded += Container_Unloaded;
                _containers.Add(containerName, container);
            }
        }

        private static void Container_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var control = sender as ContentControl;
            var containerName = GetContainerName(control);
            if (!_containers.ContainsKey(containerName))
            {
                _containers.Add(containerName, control);
            }
        }

        private static void Container_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            (sender as ContentControl).Unloaded -= Container_Unloaded;
            var containerName = GetContainerName(sender as ContentControl);
            if (!_hasContainers.Contains(containerName))
            {
                (sender as ContentControl).Loaded -= Container_Loaded;
            }
            if (string.IsNullOrEmpty(containerName)) return;
            _containers.Remove(containerName);
        }

        private static void BackButtonNamePropertyChanged(AvaloniaPropertyChangedEventArgs<string> arg)
        {
            if (Design.IsDesignMode || !arg.NewValue.HasValue) return;
            if (!_backButtons.TryGetValue(arg.NewValue.Value, out Control backBtn))
            {
                backBtn = arg.Sender as Control;
                backBtn.Unloaded += backBtn_Unloaded;
                if (backBtn is Button btn)
                {
                    btn.Click += Btn_Click;
                }
                else
                {
                    backBtn.PointerReleased += BackBtn_PointerReleased;
                }
                backBtn.IsVisible = false;
                _backButtons.Add(arg.NewValue.Value, backBtn);
            }
        }

        private static void BackButtonClick(object sender)
        {
            var backName = GetBackName(sender as Control);
            if (string.IsNullOrEmpty(backName)) return;

            Back(backName);
        }

        private static void Btn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            BackButtonClick(sender);
        }

        private static void BackBtn_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            BackButtonClick(sender);
        }

        private static void backBtn_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            (sender as Control).Unloaded -= backBtn_Unloaded;
            (sender as Control).PointerReleased -= BackBtn_PointerReleased;
            if (sender is Button btn)
            {
                btn.Click -= Btn_Click;
            }
            var backName = GetBackName(sender as Control);
            if (string.IsNullOrEmpty(backName)) return;
            _backButtons.Remove(backName);

        }

    }
}
