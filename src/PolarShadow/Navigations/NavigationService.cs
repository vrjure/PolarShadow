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

        public void Navigate(string containerName, Type viewType, INavigationParameters parameters)
        {
            if (!NavigationManager.TryGetContainer(containerName, out ContentControl container))
            {
                throw new InvalidOperationException($"Container name [{containerName}] not found");
            }

            var page = _service.GetRequiredService(viewType);
            if (container.Content is Control old && old.DataContext is INavigationNotify notify)
            {
                notify.OnUnload();
            }

            container.Content = page;

            if(page is Control newPage && newPage.DataContext is INavigationNotify newNotify)
            {
                newNotify.OnLoad();
            }
        }
    }
}
