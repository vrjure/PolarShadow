using PolarShadow.Api.Utilities;
using PolarShadow.Services;
using PolarShadow.Storage;

namespace PolarShadow.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPolarShadowService(this IServiceCollection services)
        {
            return services.AddPolarShadowDbService()
                .AddTransient<IMineResourceService>(sp => sp.GetRequiredService<IDbMineResourceService>())
                .AddTransient<IHistoryService>(sp => sp.GetRequiredService<IDbHistoryService>())
                .AddTransient<IPreferenceService>(sp => sp.GetRequiredService<IDbPreferenceService>())
                .AddTransient<ISourceService>(sp => sp.GetRequiredService<IDbSourceService>());
        }

        public static IServiceCollection AddUntities(this IServiceCollection services)
        {
            return services.AddSingleton<FileSafeOperate>();
        }
    }
}
