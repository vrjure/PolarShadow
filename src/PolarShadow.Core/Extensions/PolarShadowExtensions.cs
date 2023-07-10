using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Core
{
    public static class PolarShadowExtensions
    {
        public static T GetItem<T>(this IPolarShadow polarShadow) where T : IPolarShadowItem
        {
            return polarShadow.Items.Where(f => f is T).Cast<T>().FirstOrDefault();
        }

        public static IEnumerable<ISite> GetSites(this IPolarShadow polarShadow, Func<ISite, bool> predicate)
        {
            return GetItem<ISiteItem>(polarShadow)?.Sites?.Where(f => predicate(f));
        }
    }
}
