using LibVLCSharp.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
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
    }
}
