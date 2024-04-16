using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCache(this IServiceCollection services, FileCacheOptions fileOptions)
        {
            services.AddSingleton<IBufferCache, BufferCache>();
            services.AddMemoryCache();
            services.AddSingleton<IFileCache>(new FileCache(fileOptions));
            services.AddSingleton<IBufferCache, BufferCache>();
            if (!Directory.Exists(fileOptions.CacheFolder))
            {
                Directory.CreateDirectory(fileOptions.CacheFolder);
            }
            return services;
        }
    }
}
