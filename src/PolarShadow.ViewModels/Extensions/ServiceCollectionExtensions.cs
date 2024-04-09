using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCache(this IServiceCollection services)
        {
            return services.AddSingleton<IBufferCache, BufferCache>();
        }
    }
}
