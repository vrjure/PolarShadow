using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    internal interface IImageCache
    {
        Task<string> GetCacheUrlAsync(string imageSrc);
        Task RevokeUrlAsync(string url);
        void RemoveCache(string imageSrc);
    }
}
