using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Resources
{
    public static class PolarShadowExtensions
    {
        public static IEnumerable<ISite> GetSites(this IPolarShadow polar)
        {
            var items = polar.GetItems<ISiteItem>();
            foreach (var item in items)
            {
                foreach (var site in item.Sites)
                {
                    yield return site;
                }
            }
        }

        public static IEnumerable<ISite> GetSites(this IPolarShadow polar, string itemName)
        {
            return polar.GetItem<ISiteItem>(itemName)?.Sites;
        }


        public static bool TryGetSite(this IPolarShadow polarShadow, string siteName, out ISite site)
        {
            site = null;
            var items = polarShadow.GetItems<ISiteItem>();
            if (items == null) return false;

            foreach (var siteItem in items)
            {
                if (siteItem.TryGetSite(siteName, out site))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetSite(this IPolarShadow polar, string itemName, string siteName, out ISite site)
        {
            site = null;
            var item = polar.GetItem<ISiteItem>(itemName);
            if (item == null) return false;

            return item.TryGetSite(siteName, out site);
        }

        public static IEnumerable<ISite> GetVideoSites(this IPolarShadow polarShadow) => polarShadow.GetSites(PolarShadowItems.VideoSites);

        public static IEnumerable<ISite> GetVideoSites(this IPolarShadow polarShadow, Func<ISite, bool> predicate)
        {
            return GetVideoSites(polarShadow).Where(f => predicate(f));
        }

        public static IEnumerable<ISite> GetWebAnalysisSites(this IPolarShadow polarShadow) => polarShadow.GetSites(PolarShadowItems.WebAnalysisSites);

        public static ISiteRequestHandler CreateSiteRequestHandler(this IPolarShadow polar, ISite site, string requestName)
        {
            return site.CreateRequestHandler(polar, requestName);

        }
    }
}
