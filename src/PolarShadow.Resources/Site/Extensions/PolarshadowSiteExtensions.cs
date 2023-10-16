﻿using System;
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

        public static Task<TResponse> ExecuteAsync<TResponse>(this ISite site, IPolarShadow polar, string requestName, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(polar, requestName);
            if (handler == null) return default;
            return ExecuteAsync<TResponse>(handler, default, cancellation);
        }

        public static Task<TResponse> ExecuteAsync<TResponse>(this ISite site, IPolarShadow polar, string requestName, Action<IParameterCollection> builder, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(polar, requestName);
            if (handler == null) return default;
            return ExecuteAsync<TResponse>(handler, builder, cancellation);
        }

        public static Task<TResponse> ExecuteAsync<TRequest, TResponse>(this ISite site, IPolarShadow polar, string requestName, TRequest request, CancellationToken cancellation = default) where TRequest : class
        {
            var handler = site.CreateRequestHandler(polar, requestName);
            if (handler == null) return default;
            return ExecuteAsync<TRequest, TResponse>(handler, request, cancellation);
        }



        public static Task<string> ExecuteAsync(this ISite site, IPolarShadow polar, string requestName, string input, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(polar, requestName);
            if (handler == null) return default;
            return ExecuteAsync(handler, input, cancellation);
        }

        public static Task<string> ExecuteAsync(this ISite site, IPolarShadow polar, string requestName, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(polar, requestName);
            if (handler == null) return default;
            return ExecuteAsync(handler, cancellation);
        }

        public static Task<string> ExecuteAsync(this ISite site, IPolarShadow polar, string requestName, Action<IParameterCollection> builder, CancellationToken cancellation = default)
        {
            var handler = site.CreateRequestHandler(polar, requestName);
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



        public static ISiteRequestHandler CreateRequestHandler(this ISite site, IPolarShadow polar, string requestName)
        {
            if (!site.Requests.TryGetValue(requestName, out ISiteRequest request))
            {
                return null;

            }

            var siteItem = polar.GetItem<ISiteItem>();
            var parameterItem = polar.GetItem<IParameterItem>();

            var requestHandler = siteItem.HttpHandler;
            if (request.UseWebView.HasValue)
            {
                requestHandler = request.UseWebView.Value ? siteItem.WebViewHandler : requestHandler;
            }
            else if (site.UseWebView.HasValue)
            {
                requestHandler = site.UseWebView.Value ? siteItem.WebViewHandler : requestHandler;
            }

            if (requestHandler == null) throw new InvalidOperationException("RequestHandler not be set");

            var parameters = new Parameters();
            if (parameterItem != null && parameterItem.Parameters?.Count > 0)
            {
                parameters.Add(parameterItem.Parameters);
            }

            if (site.Parameters != null && site.Parameters.Count > 0)
            {
                parameters.Add(site.Parameters);
            }

            if (request.Parameters != null && request.Parameters.Count > 0)
            {
                parameters.Add(request.Parameters);
            }

            var siteInfo = new KeyValueParameter
            {
                { "site:name", site.Name },
                { "site:domain", site.Domain },
                { "site:request", requestName }
            };

            parameters.Add(siteInfo);

            var writings = new List<IContentWriting>();
            foreach (var item in siteItem.EnumeratorRequestRules(requestName))
            {
                if (item.Writings == null || item.Writings.Count == 0 )
                {
                    continue;
                }
                writings.AddRange(item.Writings);
            }

            return new SiteRequestHandler(site, requestHandler, request, parameters, writings);
        }
    }
}
