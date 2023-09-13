using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class PolarShadowBuilderExtensions
    {
        public static bool HasItemBuilder<T>(this IPolarShadowBuilder builder) where T : IPolarShadowItemBuilder
        {
            return builder.ItemBuilders.Any(f => f is T);
        }

        public static bool TryGetItemBuilder<T>(this IPolarShadowBuilder builder, out T value) where T : class, IPolarShadowItemBuilder
        {
            value = builder.ItemBuilders.Where(f => f is T).FirstOrDefault() as T;
            return value != null;
        }
    }
}
