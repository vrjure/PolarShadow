using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PolarShadow.Core
{
    public static class PolarShadowExtensions
    {
        public static IPolarShadow SaveTo(this IPolarShadow polarShadow, IPolarShadowSource source)
        {
            using var ms = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(ms, JsonOption.DefaultWriteOption);
            polarShadow.WriteTo(jsonWriter);
            jsonWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            source.Save(ms);
            return polarShadow;
        }

        public static IPolarShadow LoadJsonFileSource(this IPolarShadow polarShadow, string path, bool reload = false)
        {
            polarShadow.Load(new JsonFileSource { Path = path}, reload);
            return polarShadow;
        }

        public static IPolarShadow LoadJsonStreamSource(this IPolarShadow polarShadow, Stream stream, bool reload = false)
        {
            polarShadow.Load(new JsonStreamSource(stream), reload);
            return polarShadow;
        }

        public static T GetItem<T>(this IPolarShadow polarShadow) where T : IPolarShadowItem
        {
            return polarShadow.Items.Where(f => f is T).Cast<T>().FirstOrDefault();
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

        public static bool TryGetSite(this IPolarShadow polarShadow, string siteName, out ISite site)
        {
            site = null;
            var item = GetItem<ISiteItem>(polarShadow);
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
    }
}
