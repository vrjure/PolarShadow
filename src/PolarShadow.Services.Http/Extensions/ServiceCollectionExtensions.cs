using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services.Http
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPolarShadowHttpService(this IServiceCollection services, Action<IServiceProvider, HttpClient>? configureClient = null)
        {
            if (configureClient != null)
            {
                services.AddHttpClient<IHttpMineResourceService, MineResourceService>(configureClient);
                services.AddHttpClient<IHttpHistoryService, HistoryService>(configureClient);
                services.AddHttpClient<IHttpPreferenceService, PreferenceService>(configureClient);
                services.AddHttpClient<IServerService, ServerService>(configureClient);
                services.AddHttpClient<IHttpSourceService, SourceService>(configureClient);
            }
            return services;
        }
    }
}
