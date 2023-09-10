using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using PolarShadow.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal class HttpCachingHandler : DelegatingHandler
    {
        private readonly IBufferCache _cache;
        public HttpCachingHandler(IBufferCache memoryCache) :this(memoryCache, new HttpClientHandler())
        {

        }
        public HttpCachingHandler(IBufferCache memoryCache, HttpMessageHandler httpMessageHandler)
        {
            _cache = memoryCache;
            InnerHandler = httpMessageHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var key = BufferCache.SHA(request.RequestUri.ToString());

            if (_cache.ContainsKey(key))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(await _cache.GetAsync(key))
                };
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            return response;

            var data = await response.Content.ReadAsByteArrayAsync();
            await _cache.SetAsync(key, data, BufferLocation.Memory);
            return response;
        }
    }
}
