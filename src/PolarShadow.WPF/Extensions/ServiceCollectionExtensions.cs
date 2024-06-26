﻿using CommunityToolkit.Mvvm.ComponentModel;
using PolarShadow.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection RegisterSingletonViewWithModel<TView, TViewModel>(this IServiceCollection service) where TView : FrameworkElement,new() where TViewModel : ObservableObject
        {
            NavigationManager.Add(typeof(TViewModel), typeof(TView));
            return service.AddSingleton<TViewModel>().AddSingleton<TView>();
        }
        
        public static IServiceCollection RegisterTransientViewWithModel<TView, TViewModel>(this IServiceCollection service) where TView: FrameworkElement, new() where TViewModel : ObservableObject
        {
            NavigationManager.Add(typeof(TViewModel), typeof(TView));
            return service.AddTransient<TViewModel>().AddTransient<TView>();
        }

        public static IServiceCollection RegisterScopedViewWithModel<TView, TViewModel>(this IServiceCollection service) where TView : FrameworkElement, new() where TViewModel : ObservableObject
        {
            NavigationManager.Add(typeof(TViewModel), typeof(TView));
            return service.AddScoped<TViewModel>().AddScoped<TView>();
        }
    }
}
