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

        public static bool HasRequest(this ISite site, string requestName)
        {
            return site[requestName] != null;
        }

        public static bool TryGetRequest(this ISite site, string requestName, out ISiteRequest request)
        {
            request = site[requestName];
            return request != null;
        }

        public static async Task<TResponse> ExecuteAsync<TRequest, TResponse>(this ISite site, string name, TRequest request, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(name);
            var input = JsonSerializer.Serialize(request, JsonOption.DefaultSerializer);
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(input, ms, cancellation).ConfigureAwait(false);
            return JsonSerializer.Deserialize<TResponse>(ms, JsonOption.DefaultSerializer);
        }

        public static async Task<string> ExecuteAsync(this ISite site, string name, string input, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(name);
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(input, ms, cancellation).ConfigureAwait(false);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static async Task<string> ExecuteAsync(this ISite site, string name, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(name);
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(ms, cancellation).ConfigureAwait(false);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }
    }
}
