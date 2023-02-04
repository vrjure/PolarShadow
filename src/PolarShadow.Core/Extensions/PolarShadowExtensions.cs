using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PolarShadow.Core
{
    public static class PolarShadowExtensions
    {
        public static bool HasAbility(this IPolarShadowSite site, string abilityName)
        {
            return site.EnumerableAbilities().Any(f=>f.Name.Equals(abilityName));
        }

        public static IAnalysisAbility GetAbility(this IPolarShadowSite site, string abilityName)
        {
            return site.EnumerableAbilities().Where(f=>f.Name.Equals(abilityName)).FirstOrDefault();
        }
        public static IEnumerable<IPolarShadowSite> GetAbilitieSites<T>(this IPolarShadow ps) where T : IAnalysisAbility
        {
            return ps.GetSites().Where(f => f.EnumerableAbilities().Any(f => f is T));
        }

        public static bool TryGetAbility<T>(this IPolarShadowSite site, out T ability) where T : IAnalysisAbility
        {
            var result = site.EnumerableAbilities().FirstOrDefault(f => f is T);
            ability = (T)result;
            return result == default;
        }

        public static IPolarShadowBuilder AddAbility<T>(this IPolarShadowBuilder builder) where T : IAnalysisAbility, new()
        {
            builder.AddAbility(new T());
            return builder;
        }

        public static IPolarShadowBuilder AddDefaultAbilities(this IPolarShadowBuilder builder)
        {
            return builder.AddAbility<SearchAbleDefault>()
                .AddAbility<GetDetailAbleDefault>()
                .AddAbility<AnalysisSourceAbleDefault>();
        }
    }
}
