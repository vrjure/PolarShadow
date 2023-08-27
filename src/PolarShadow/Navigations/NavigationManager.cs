using Avalonia;
using Avalonia.Controls;
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

        static NavigationManager()
        {
            ContainerNameProperty.Changed.Subscribe(HandlerContainerNameChanged);
        }

        public static bool TryGetContainer(string containerName, out ContentControl container)
        {
            return _containers.TryGetValue(containerName, out container);
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

        private static void HandlerContainerNameChanged(AvaloniaPropertyChangedEventArgs<string> args)
        {
            if (Design.IsDesignMode || !args.NewValue.HasValue) return;
            if (!_containers.TryGetValue(args.NewValue.Value, out ContentControl container))
            {
                container = args.Sender as ContentControl;
                container.Unloaded += Container_Unloaded;
                _containers.Add(args.NewValue.Value, container);
            }
        }

        private static void Container_Unloaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            (sender as ContentControl).Unloaded -= Container_Unloaded;

            var containerName = GetContainerName(sender as ContentControl);
            if (string.IsNullOrEmpty(containerName)) return;
            _containers.Remove(containerName);
        }
    }
}
