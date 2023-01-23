using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PolarShadow.Core
{
    public static class PolarShadowExtensions
    {
        public static IEnumerable<T> GetAbilities<T>(this IPolarShadow ps, string abilityName)
        {
            return ps.GetSites().Where(f=>f.HasAbility(abilityName)).Select(f=> (T)f.GetAbility(abilityName));
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
