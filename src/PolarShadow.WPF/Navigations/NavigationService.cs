using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PolarShadow.Navigations
{
    class NavigationService : INavigationService
    {
        private readonly IServiceProvider _services;
        public NavigationService(IServiceProvider services)
        {
            _services = services;
        }
        public bool CanBack(string container)
        {
            if (!NavigationManager.CanBack(container, out Stack<FrameworkElement> _)) return false;
            return true;
        }

        public void Back(string container)
        {
            NavigationManager.Back(container);
        }

        public void Navigate(string containerName, Type viewModelType, IDictionary<string, object> parameters, bool canBack)
        {
            if (!NavigationManager.TryGetContainer(containerName, out ContentControl _))
            {
                throw new InvalidOperationException($"Container name [{containerName}] not found");
            }

            if (!NavigationManager.TryGetView(viewModelType, out Type view))
            {
                throw new InvalidOperationException($"Can not found [{viewModelType.GetType()}] not found");
            }

            var page = _services.GetRequiredService(view) as Control;
            NavigationManager.Navigate(containerName, page, parameters, canBack);
        }
    }
}
