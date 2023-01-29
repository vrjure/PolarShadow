using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PolarShadow.Core
{
    public static class PolarShadowExtensions
    {
        public static IEnumerable<T> GetAbilitySites<T>(this IPolarShadow ps, string abilityName)
        {
            return ps.GetSites().Where(f=>f.HasAbility(abilityName)).Select(f=> (T)f.GetAbility(abilityName));
        }

        public static bool TryGetAbility<T>(this IPolarShadowSite site, out T ability)
        {
            ability = default;
            if (!PolarShadowBuilder.SupportAbilityTypes.TryGetValue(typeof(T), out string abilityName))
            {
                return false;
            }

            if (!site.HasAbility(abilityName))
            {
                return false;
            }

            ability = (T)site.GetAbility(abilityName);
            if (ability == null)
            {
                return false;
            }
            return true;
        }

        public static IPolarShadowBuilder RegisterSupportAbilityFactory<T>(this IPolarShadowBuilder builder, string name)
        {
            builder.RegisterSupportAbilityFactory(name, new AbilityFactoryDefault<T>());
            return builder;
        }

        public static IPolarShadowBuilder AutoSite(this IPolarShadowBuilder builder, Assembly assembly)
        {
            //var types = assembly.GetExportedTypes();
            //foreach (var item in types)
            //{
            //    var isSite = item.GetInterface(nameof(IPolarShadowSite));
            //    if (isSite == null)
            //    {
            //        continue;
            //    }

            //    var npc = item.GetConstructor(Type.EmptyTypes);
            //    if (npc == null)
            //    {
            //        continue;
            //    }

            //    builder.AddSite((IPolarShadowSite)Activator.CreateInstance(item));
            //}

            return builder;
        }
    }
}
