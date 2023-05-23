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
