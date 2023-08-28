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
        public static IServiceCollection RegisterSingletonView<TView>(this IServiceCollection service) where TView : Control
        {
            return service.AddSingleton<TView>();
        }

        public static IServiceCollection RegisterTransientView<TView>(this IServiceCollection service) where TView : Control
        {
            return service.AddTransient<TView>();
        }

        public static IServiceCollection RegisterScopedView<TView>(this IServiceCollection service) where TView: Control
        {
            return service.AddScoped<TView>();
        }

        public static IServiceCollection RegisterSingletonViewModel<TViewModel>(this IServiceCollection service) where TViewModel : ObservableObject
        {
            return service.AddSingleton<TViewModel>();
        }

        public static IServiceCollection RegisterTransientViewModel<TViewModel>(this IServiceCollection service) where TViewModel : ObservableObject
        {
            return service.AddTransient<TViewModel>();
        }

        public static IServiceCollection RegisterScopedViewModel<TViewModel>(this IServiceCollection service) where TViewModel : ObservableObject
        {
            return service.AddScoped<TViewModel>();
        }

        public static IServiceCollection RegisterSingletonViewWithModel<TView, TViewModel>(this IServiceCollection service) where TView : Control,new() where TViewModel : ObservableObject
        {
            return service.AddSingleton<TViewModel>().AddSingleton<TView>(sp =>
            {
                var viewModel = sp.GetService<TViewModel>();
                return new TView() { DataContext = viewModel };
            });
        }
        
        public static IServiceCollection RegisterTransientViewWithModel<TView, TViewModel>(this IServiceCollection service) where TView: Control, new() where TViewModel : ObservableObject
        {
            return service.AddTransient<TViewModel>().AddTransient<TView>(sp =>
            {
                var viewModel = sp.GetService<TViewModel>();
                return new TView() { DataContext = viewModel };
            });
        }

        public static IServiceCollection RegisterScopedViewWithModel<TView, TViewModel>(this IServiceCollection service) where TView : Control, new() where TViewModel : ObservableObject
        {
            return service.AddScoped<TViewModel>().AddScoped<TView>(sp =>
            {
                var viewModel = sp.GetService<TViewModel>();
                return new TView() { DataContext = viewModel };
            });
        }
    }
}
