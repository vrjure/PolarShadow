using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PolarShadow.Navigations
{
    internal partial class NavigationManager
    {
        private static Dictionary<string, ContentControl> _containers = new Dictionary<string, ContentControl>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, FrameworkElement> _backButtons = new Dictionary<string, FrameworkElement>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, Stack<FrameworkElement>> _history = new Dictionary<string, Stack<FrameworkElement>>();

        public static bool TryGetContainer(string containerName, out ContentControl container)
        {
            return _containers.TryGetValue(containerName, out container);
        }

        public static bool TryGetBackButton(string backName, out FrameworkElement input)
        {
            return _backButtons.TryGetValue(backName, out input);
        }

        public static bool CanBack(string container, out Stack<FrameworkElement> his)
        {
            if (!_history.TryGetValue(container, out his)) return false;
            if (his.Count <= 0) return false;

            return true;
        }

        public static void Back(string container)
        {
            if (!CanBack(container, out Stack<FrameworkElement> his)) return;

            var view = his.Pop();

            Navigate(container, view, default, false);
        }

        public static void Navigate(string containerName, FrameworkElement page, IDictionary<string, object> parameters, bool canBack)
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
                    if (!_history.TryGetValue(containerName, out Stack<FrameworkElement> his))
                    {
                        his = new Stack<FrameworkElement>();
                        _history[containerName] = his;
                    }
                    his.Push(current);
                }
            }

            if (parameters != null && page.DataContext is IParameterObtain po)
            {
                po.ApplyParameter(parameters);
            }
            container.Content = null;//unload first
            container.Content = page;

            if (TryGetBackButton(containerName, out FrameworkElement btn))
            {
                btn.Opacity = CanBack(containerName, out Stack<FrameworkElement> _) ? 1 : 0;
            }

        }
    }
}
