using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Resources
{
    public static class PolarShadowExtensions
    {
        public static IEnumerable<ISite> GetSites(this IPolarShadow polarShadow)
        {
            var item = polarShadow.GetItem<ISiteItem>();
            if (item == null)
            {
                return Enumerable.Empty<ISite>();
            }

            return item.Sites;
        }

        public static IEnumerable<ISite> GetSites(this IPolarShadow polarShadow, Func<ISite, bool> predicate)
        {
            return GetSites(polarShadow).Where(f => predicate(f));
        }

        public static bool TryGetSite(this IPolarShadow polarShadow, string siteName, out ISite site)
        {
            site = null;
            var item = polarShadow.GetItem<ISiteItem>();
            if (item == null) return false;

            return item.TryGetSite(siteName, out site);
        }

        public static IPolarShadow AddOrUpdateSite(this IPolarShadow polarShadow, string siteName, Action<ISite> siteBuilder)
        {
            var siteItem = polarShadow.GetItem<ISiteItem>();
            if (siteItem == null)
            {
                throw new ArgumentException("ISiteItem not exist");
            }
            siteItem.AddOrUpdateSite(siteName, siteBuilder);

            return polarShadow;
        }

        public static IEnumerable<WebAnalysisSource> GetAnalysisSources(this IPolarShadow polarShadow)
        {
            return polarShadow.GetItem<IWebAnalysisItem>()?.Sources;
        }
    }
}
