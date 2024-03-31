using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public static class ResourceServiceExtensions
    {
        public static T FindResource<T>(this IResourceService resourceService, string key)
        {
            return (T)resourceService.FindResource(key);
        }
    }
}
