using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Services;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    public static class PreferenceServiceExtensions
    {
        public static bool HasServerAddress(this IServiceProvider services)
        {
            var service = services.GetRequiredService<IDbPreferenceService>();
            return service.HasServerAddress();
        }

        public static bool HasServerAddress(this IDbPreferenceService services)
        {
            return services.Get(Preferences.ApiEnable, false);
        }
    }
}
