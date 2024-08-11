using LibVLCSharp.Shared;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Essentials;
using PolarShadow.Services.Http;
using PolarShadow.Services;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.ViewModels.Services;

namespace PolarShadow
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPolarShadowService(this IServiceCollection services) 
        {
            return services.AddPolarShadowDbService()
                .AddPolarShadowHttpService((sp, client) =>
                {
                    var preference = sp.GetRequiredService<IDbPreferenceService>();
                    if (preference.HasServerAddress())
                    {
                        client.BaseAddress = new Uri(preference.Get(Preferences.ServerAddress, ""));
                    }
                })
                .AddTransient<IMineResourceService>(sp =>
                {
                    if (sp.HasServerAddress())
                    {
                        return sp.GetRequiredService<IHttpMineResourceService>();
                    }
                    else
                    {
                        return sp.GetRequiredService<IDbMineResourceService>();
                    }
                })
                .AddTransient<IHistoryService>(sp =>
                {
                    if (sp.HasServerAddress())
                    {
                        return sp.GetRequiredService<IHttpHistoryService>();
                    }
                    else
                    {
                        return sp.GetRequiredService<IDbHistoryService>();
                    }
                })
                .AddTransient<IPreferenceService>(sp =>
                {
                    var preference = sp.GetRequiredService<IDbPreferenceService>();
                    if (preference.HasServerAddress())
                    {
                        return sp.GetRequiredService<IHttpPreferenceService>();
                    }
                    else
                    {
                        return preference;
                    }
                })
                .AddTransient<ISourceService>(sp =>
                {
                    if (sp.HasServerAddress())
                    {
                        return sp.GetRequiredService<IHttpSourceService>();
                    }
                    else
                    {
                        return sp.GetRequiredService<IDbSourceService>();
                    }
                })
                .AddTransient(typeof(ISyncService<>), typeof(SyncService<>));
        }
    }
}
