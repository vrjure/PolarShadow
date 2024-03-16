using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    public class FileCacheOptions
    {
        public string CacheFolder {  get; set; }
        public long MaxCacheSize { get; set; } = 30 * 1024 * 1024;
    }
}
