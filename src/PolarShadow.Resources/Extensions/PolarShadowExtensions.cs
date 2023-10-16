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

        public static IEnumerable<WebAnalysisSource> GetAnalysisSources(this IPolarShadow polarShadow)
        {
            return polarShadow.GetItem<IWebAnalysisItem>()?.Sources;
        }

        public static ISiteRequestHandler CreateSiteRequestHandler(this IPolarShadow polar, ISite site, string requestName)
        {
            return site.CreateRequestHandler(polar, requestName);

        }
    }
}
