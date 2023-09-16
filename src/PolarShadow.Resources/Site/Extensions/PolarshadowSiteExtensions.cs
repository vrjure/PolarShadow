using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PolarShadow.Core;

namespace PolarShadow.Resources
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
            if (site.Requests == null)
            {
                return false;
            }
            return site.Requests.ContainsKey(requestName);
        }

        public static bool TryGetRequest(this ISite site, string requestName, out ISiteRequest request)
        {
            request = default;
            if (site.Requests == null)
            {
                return false;
            }
            return site.Requests.TryGetValue(requestName, out request);
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
            JsonSerializer.Serialize(jsonWriter, request, JsonOption.DefaultSerializer);
            jsonWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static async Task ExecuteAsync(this ISiteRequestHandler handler, Stream stream, CancellationToken cancellation = default)
        {
            await handler.ExecuteAsync(stream, default, cancellation);
        }

        public static async Task ExecuteAsync(this ISiteRequestHandler handler, Stream stream, Action<IParameterCollection> parameters, CancellationToken cancellation = default)
        {
            await handler.ExecuteAsync(stream, parameters, cancellation);
        }

        public static async Task<TResponse> ExecuteAsync<TResponse>(this ISiteRequestHandler handler, CancellationToken cancellation = default)
        {
            if (handler == null) return default;
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(ms, default, cancellation).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.Deserialize<TResponse>(ms, JsonOption.DefaultSerializer);
        }

        public static async Task<TResponse> ExecuteAsync<TResponse>(this ISiteRequestHandler handler, Action<IParameterCollection> builder = default, CancellationToken cancellation = default)
        {
            if (handler == null) return default;
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(ms, builder, cancellation).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.Deserialize<TResponse>(ms, JsonOption.DefaultSerializer);
        }

        public static async Task<TResponse> ExecuteAsync<TRequest, TResponse>(this ISiteRequestHandler handler, TRequest request, CancellationToken cancellation = default) where TRequest : class
        {
            if (handler == null) return default;
            using var ms = new MemoryStream();
            await handler.ExecuteAsync(ms, builder => 
            { 
                builder.AddObjectValue(request);
            }, cancellation).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.Deserialize<TResponse>(ms, JsonOption.DefaultSerializer);
        }

        public static Task<TResponse> ExecuteAsync<TResponse>(this ISite site, string requestName, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(requestName);
            if (handler == null) return default;
            return ExecuteAsync<TResponse>(handler, default, cancellation);
        }

        public static Task<TResponse> ExecuteAsync<TResponse>(this ISite site, string requestName, Action<IParameterCollection> builder, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(requestName);
            if (handler == null) return default;
            return ExecuteAsync<TResponse>(handler, builder, cancellation);
        }

        public static Task<TResponse> ExecuteAsync<TRequest, TResponse>(this ISite site, string requestName, TRequest request, CancellationToken cancellation = default) where TRequest : class
        {
            var handler = site.CreateRequestHandler(requestName);
            if (handler == null) return default;
            return ExecuteAsync<TRequest, TResponse>(handler, request, cancellation);
        }


        public static Task<string> ExecuteAsync(this ISite site, string requestName, string input, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(requestName);
            if (handler == null) return default;
            return ExecuteAsync(handler, input, cancellation);
        }

        public static Task<string> ExecuteAsync(this ISite site, string requestName, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(requestName);
            if (handler == null) return default;
            return ExecuteAsync(handler, cancellation);
        }

        public static Task<string> ExecuteAsync(this ISite site, string requestName, Action<IParameterCollection> builder, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(requestName);
            if (handler == null) return default;
            return ExecuteAsync(handler, builder, cancellation);
        }

        public static async Task<string> ExecuteAsync(this ISiteRequestHandler request, CancellationToken cancellation = default)
        {
            if (request == null) return default;
            using var ms = new MemoryStream();
            await request.ExecuteAsync(ms, cancellation).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static async Task<string> ExecuteAsync(this ISiteRequestHandler request, Action<IParameterCollection> builder, CancellationToken cancellation = default)
        {
            if (request == null) return default;
            using var ms = new MemoryStream();
            await request.ExecuteAsync(ms, builder, cancellation).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static async Task<string> ExecuteAsync(this ISiteRequestHandler request, string input, CancellationToken cancellation = default)
        {
            if (request == null) return default;
            using var ms = new MemoryStream();
            await request.ExecuteAsync(ms, builder =>
            {
                var objectValue = new ObjectParameter();
                using var doc = JsonDocument.Parse(input);
                objectValue.Add(doc.RootElement.Clone());
                builder.Add(objectValue);
            }, cancellation).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }
    }
}
