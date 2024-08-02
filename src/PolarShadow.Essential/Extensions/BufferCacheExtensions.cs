using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public static class BufferCacheExtensions
    {
        public static async Task CacheFileIfExisedInMemory(this IBufferCache bufferCache, string key)
        {
            if (bufferCache.ContainsKey(key, BufferLocation.Memory))
            {
                await bufferCache.SetAsync(key, bufferCache.Get(key, BufferLocation.Memory), BufferLocation.File);
            }
        }
    }
}
