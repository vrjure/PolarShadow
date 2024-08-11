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
        public static IServiceCollection AddPolarShadowDbService(this IServiceCollection services)
        {
            return services.AddTransient<IDbMineResourceService, MineResourceService>()
                .AddTransient<IDbHistoryService, HistoryService>()
                .AddTransient<IDbPreferenceService, PreferenceService>()
                .AddTransient<IDbSourceService, SourceService>()
                .AddMemoryCache();
        }
    }
}
