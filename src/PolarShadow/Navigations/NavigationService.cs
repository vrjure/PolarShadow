using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public class NavigationService : INavigationService
    {

        private readonly IServiceProvider _service;
        public NavigationService(IServiceProvider service)
        {
            _service = service;
        }

        public bool CanBack(string container)
        {
            if (!NavigationManager.CanBack(container, out Stack<Control> _)) return false;
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

            var page = _service.GetRequiredService(view) as Control;
            NavigationManager.Navigate(containerName, page, parameters, canBack);
        }
    }
}
