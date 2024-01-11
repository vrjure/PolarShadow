using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.Controls
{
    public sealed class NativeControls
    {
        private static IServiceCollection _services;
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static void ConfigureService(IServiceCollection services)
        {
            _services = services;
            _services.AddLibVLC();
        }

        public static IServiceCollection AddHandler<THandler, THandlerImp>() where THandlerImp: class,THandler where THandler :class
        {
            return _services.AddTransient<THandler, THandlerImp>();
        }


        public static THandler GetHandler<THandler>()
        {
            return _serviceProvider.GetService<THandler>();
        }
    }
}
