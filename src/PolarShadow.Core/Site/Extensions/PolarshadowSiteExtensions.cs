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
        public static async Task<TResponse> ExecuteAsync<TRequest, TResponse>(this IPolarShadowSite site, string name, TRequest request, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(name);
            var input = JsonSerializer.Serialize(request, JsonOption.DefaultSerializer);
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(input, ms, cancellation).ConfigureAwait(false);
            return JsonSerializer.Deserialize<TResponse>(ms, JsonOption.DefaultSerializer);
        }

        public static async Task<string> ExecuteAsync(this IPolarShadowSite site, string name, string input, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(name);
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(input, ms, cancellation).ConfigureAwait(false);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static async Task<string> ExecuteAsync(this IPolarShadowSite site, string name, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(name);
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(ms, cancellation).ConfigureAwait(false);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }
    }
}
