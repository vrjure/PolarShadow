using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStorageServices(this IServiceCollection services)
        {
            services.AddSingleton<IMyCollectionService, MyCollectionService>();
            services.AddSingleton<IWatchRecordService, WatchRecordService>();
            return services;
        }
    }
}
