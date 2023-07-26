using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public static class PolarshadowSiteExtensions
    {
        public static bool HasSite(this ISiteItem sites, string siteName)
        {
            return sites[siteName] != null;
        }

        public static bool TryGetSite(this ISiteItem sites, string siteName, out ISite site)
        {
            site = sites[siteName];
            return site != null;
        }

        public static ISiteItem AddOrUpdateSite(this ISiteItem item, string siteName, Action<ISite> siteBuilder)
        {
            if (string.IsNullOrEmpty(siteName) || string.IsNullOrWhiteSpace(siteName))
            {
                throw new ArgumentException("site name must be set", nameof(siteName));
            }

            if (!item.TryGetSite(siteName, out ISite site))
            {
                site = new SiteDefault();
            }
            siteBuilder(site);
            item[siteName] = site;
            return item;
        }

        public static bool HasRequest(this ISite site, string requestName)
        {
            return site.Requests.ContainsKey(requestName);
        }

        public static bool TryGetRequest(this ISite site, string requestName, out ISiteRequest request)
        {
            return site.TryGetRequest(requestName, out request);
        }

        public static ISite AddOrUpdateRequest(this ISite site, string requestName, Action<ISiteRequest> requestBuilder)
        {
            if (string.IsNullOrEmpty(requestName) || string.IsNullOrWhiteSpace(requestName))
            {
                throw new ArgumentException("request name must be set", nameof(requestName));
            }

            if (HasRequest(site, requestName))
            {
                throw new ArgumentException($"{requestName} exist in site [{site.Name}]");
            }

            var request = new SiteRequest();
            requestBuilder(request);

            site.Requests[requestName] = request;
            return site;
        }

        public static string GetRequestJson(this ISiteRequest request, JsonWriterOptions options = default)
        {
            using var ms = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(ms, options);
            request.WriteTo(jsonWriter);
            jsonWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static async Task ExecuteAsync(this ISiteRequestHandler handler, Stream stream, CancellationToken cancellation = default)
        {
            await handler.ExecuteAsync(default, stream, cancellation);
        }

        public static async Task<TResponse> ExecuteAsync<TRequest, TResponse>(this ISite site, string requestName, TRequest request, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(requestName);
            if (handler == null) return default;
            var input = JsonSerializer.Serialize(request, JsonOption.DefaultSerializer);
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(input, ms, cancellation).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.Deserialize<TResponse>(ms, JsonOption.DefaultSerializer);
        }

        public static async Task<string> ExecuteAsync(this ISite site, string requestName, string input, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(requestName);
            if (handler == null) return default;
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(input, ms, cancellation).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static async Task<string> ExecuteAsync(this ISite site, string requestName, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(requestName);
            if (handler == null) return default;
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(ms, cancellation).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }
    }
}
