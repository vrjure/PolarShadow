using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PolarShadow.Core
{
    public static class PolarShadowExtensions
    {
        public static IEnumerable<T> GetSites<T>(this IPolarShadow polarShadow)
        {
            return polarShadow.GetSites().Where(f => f is T).Cast<T>();
        }

        public static IPolarShadowBuilder AutoSite(this IPolarShadowBuilder builder, Assembly assembly)
        {
            var types = assembly.GetExportedTypes();
            foreach (var item in types)
            {
                var isSite = item.GetInterface(nameof(IPolarShadowSite));
                if (isSite == null)
                {
                    continue;
                }

                var npc = item.GetConstructor(Type.EmptyTypes);
                if (npc == null)
                {
                    continue;
                }

                builder.AddSite((IPolarShadowSite)Activator.CreateInstance(item));
            }

            return builder;
        }
    }
}
