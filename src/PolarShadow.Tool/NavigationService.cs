using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PolarShadow.Tool
{
    static class NavigationService 
    {
        public static bool Navigate<T>(this FrameworkElement ui, string containerName) where T : FrameworkElement
        {
            return Navigate<T>(ui, containerName, null);
        }
        public static bool Navigate<T>(this FrameworkElement ui, string containerName, IDictionary<string, object> paramaters) where T : FrameworkElement
        {
            var control = ui.FindName(containerName);
            if (control is ContentControl content)
            {
                var page = App.Current.Services.GetRequiredService<T>();
                if (page.DataContext is IReferenceUI referenceUI)
                {
                    referenceUI.UI = page;
                }

                content.Content = page;

                if (paramaters != null && page.DataContext is IQueryAttributable query)
                {
                    query.ApplyQuery(paramaters);
                }
                return true;
            }
            return false;
        }

        public static bool Refresh(this FrameworkElement ui, string containerName)
        {
            var control = ui.FindName(containerName);
            if (control is ContentControl content)
            {
                var contentControl = content.Content as FrameworkElement;
                if (contentControl == null)
                {
                    return false;
                }

                object saveObject = null;
                if (contentControl.DataContext is IContextStorage context)
                {
                    saveObject = context.Save();
                }
                var page = App.Current.Services.GetRequiredService(contentControl.GetType()) as FrameworkElement;
                if (page.DataContext is IReferenceUI referenceUI)
                {
                    referenceUI.UI = page;
                }

                if (page.DataContext is IContextStorage newContext)
                {
                    newContext.Apply(saveObject);
                }
                content.Content = page;

                return true;
            }

            return false;
        }
    }
}
