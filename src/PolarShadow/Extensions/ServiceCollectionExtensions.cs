using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class ServiceCollectionExtensions
    {

        public static IServiceCollection RegisterSingletonViewWithModel<TView, TViewModel>(this IServiceCollection service) where TView : Control,new() where TViewModel : ObservableObject
        {
            NavigationManager.Add(typeof(TViewModel), typeof(TView));
            return service.AddSingleton<TViewModel>().AddSingleton<TView>();
        }
        
        public static IServiceCollection RegisterTransientViewWithModel<TView, TViewModel>(this IServiceCollection service) where TView: Control, new() where TViewModel : ObservableObject
        {
            NavigationManager.Add(typeof(TViewModel), typeof(TView));
            return service.AddTransient<TViewModel>().AddTransient<TView>();
        }

        public static IServiceCollection RegisterScopedViewWithModel<TView, TViewModel>(this IServiceCollection service) where TView : Control, new() where TViewModel : ObservableObject
        {
            NavigationManager.Add(typeof(TViewModel), typeof(TView));
            return service.AddScoped<TViewModel>().AddScoped<TView>();
        }
    }
}
