using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class PolarShadowExtensions
    {
        public static bool TryGetAbility<T>(this IPolarShadow polarShadow, out T ability) where T : IAnalysisAbility
        {
            ability = (T)polarShadow.GetAbilities().Where(f => f is T).FirstOrDefault();
            return ability != null;
        }

        public static ICollection<IPolarShadowSite> GetAbilitySites<T>(this IPolarShadow polarShadow) where T : IAnalysisAbility
        {
            if (TryGetAbility(polarShadow, out T ability))
            {
                return polarShadow.GetSites().Where(f => f.HasAbility(ability.Name)).ToList();
            }
            return default;
        }

        public static bool TryGetSiteWithAbility<T>(this IPolarShadow polarShadow, string siteName, out IPolarShadowSite site, out T ability) where T: IAnalysisAbility
        {
            ability = default;
            return polarShadow.TryGetSite(siteName, out site) && polarShadow.TryGetAbility(out ability) && site.HasAbility(ability.Name);
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
                .AddAbility<WebAnalysisAbleDefault>();
        }

        public static IPolarShadowBuilder ImportFrom(this IPolarShadowBuilder builder, Stream jsonStream)
        {
            var option = JsonSerializer.Deserialize<PolarShadowOption>(jsonStream, JsonOption.DefaultSerializer);
            if (option.AnalysisSources != null && option.AnalysisSources.Count > 0)
            {
                foreach (var item in option.AnalysisSources)
                {
                    builder.Option.AnalysisSources.Add(item);
                }
            }

            if (option.Sites != null && option.Sites.Count > 0)
            {
                foreach (var item in option.Sites)
                {
                    builder.Option.Sites.Add(item);
                }
            }
            builder.Option.IsChanged = true;

            return builder;
        }
    }
}
