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

        public static void ConfigureService(IServiceCollection services)
        {
            _services = services;
            _services.AddLibVLC();
        }

        public static IServiceCollection AddHandler<THandler, THandlerImp>() where THandlerImp: class,THandler where THandler :class
        {
            return _services.AddTransient<THandler, THandlerImp>();
        }
    }
}
