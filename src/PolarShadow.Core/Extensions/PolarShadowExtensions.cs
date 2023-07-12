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
            return GetItems<T>(polarShadow).FirstOrDefault();
        }

        public static IEnumerable<T> GetItems<T>(this IPolarShadow polarShadow) where T: IPolarShadowItem
        {
            return polarShadow.Items.Where(f => f is T).Cast<T>();
        }

        public static IEnumerable<ISite> GetSites(this IPolarShadow polarShadow, Func<ISite, bool> predicate)
        {
            var items = GetItems<ISiteItem>(polarShadow);
            foreach (var item in items)
            {
                foreach (var site in item.Sites.Where(f=> predicate(f)))
                {
                    yield return site;
                }
            }
        }
    }
}
