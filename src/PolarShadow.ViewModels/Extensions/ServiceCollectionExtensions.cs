using LibVLCSharp.Shared;
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
        public static IServiceCollection RegisterDbPreference(this IServiceCollection services)
        {
            return services.AddSingleton<IPreference, DbPreference>();
        }
    }
}
