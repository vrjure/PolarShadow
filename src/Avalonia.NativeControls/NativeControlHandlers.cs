using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.NativeControls
{
    public sealed class NativeControlHandlers
    {
        private static readonly IServiceCollection _services = new ServiceCollection();
        private static IServiceProvider _serviceProvider;

        public static void Initialize()
        {
            _serviceProvider = _services.BuildServiceProvider();
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
