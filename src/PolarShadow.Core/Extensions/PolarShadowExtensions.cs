using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Core
{
    public static class PolarShadowExtensions
    {
        public static IPolarShadow LoadJsonFileSource(this IPolarShadow polarShadow, string path)
        {
            polarShadow.Load(new JsonFileSource { Path = path});
            return polarShadow;
        }

        public static T GetItem<T>(this IPolarShadow polarShadow) where T : IPolarShadowItem
        {
            return polarShadow.Items.Where(f =>  f is T).Cast<T>().FirstOrDefault();
        }
        
        public static IEnumerable<ISite> GetSites(this IPolarShadow polarShadow)
        {
            var item = GetItem<ISiteItem>(polarShadow);
            if (item == null)
            {
                return Enumerable.Empty<ISite>();
            }

            return item.Sites;
        }

        public static IEnumerable<ISite> GetSites(this IPolarShadow polarShadow, Func<ISite, bool> predicate)
        {
            return GetSites(polarShadow).Where(f=> predicate(f));
        }

        public static IPolarShadow AddSite(this IPolarShadow polarShadow, string siteName, Action<ISite> siteBuilder)
        {
            var siteItem = polarShadow.GetItem<ISiteItem>();
            if (siteItem == null)
            {
                throw new ArgumentException("ISiteItem not exist");
            }
            siteItem.AddSite(siteName, siteBuilder);

            return polarShadow;
        }
    }
}
