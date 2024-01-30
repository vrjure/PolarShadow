using LibVLCSharp.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Controls
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLibVLC(this IServiceCollection service, params string[] options)
        {
            return service.AddSingleton(sp =>
            {
                return new LibVLC(options);
            });
        }

        public static IServiceCollection AddWebViewOptions(this IServiceCollection service, WebViewOptions options)
        {
            return service.AddSingleton(options);
        }
    }
}
