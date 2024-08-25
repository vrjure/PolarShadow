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
        public static IServiceCollection AddPolarShadowHttpService(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Func<IServiceProvider, TokenClientOptions> tokenClientOption)
        {
            services.AddMemoryCache();
            services.AddSingleton(sp => tokenClientOption(sp));
            services.AddHttpClient<IPolarShadowTokenClient, PolarShadowTokenClient>(configureClient);

            services.AddTransient<AuthenticationHandler>();
            services.AddHttpClient<IPolarShadowClient, PolarShadowClient>(configureClient)
                .AddHttpMessageHandler<AuthenticationHandler>();

            services.AddTransient<IHttpMineResourceService, MineResourceService>();
            services.AddTransient<IHttpHistoryService, HistoryService>();
            services.AddTransient<IHttpPreferenceService, PreferenceService>();
            services.AddTransient<IServerService, ServerService>();
            services.AddTransient<IHttpSourceService, SourceService>();
            return services;
        }
    }
}
