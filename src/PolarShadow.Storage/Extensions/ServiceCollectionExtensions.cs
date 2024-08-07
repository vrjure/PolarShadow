using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterStorageService(this IServiceCollection services)
        {
            services.AddSingleton<IMineResourceService, MineResourceService>();
            services.AddSingleton<IHistoryService, HistoryService>();
            services.AddSingleton<IPreferenceService, PreferenceService>();
            return services;
        }
    }
}
