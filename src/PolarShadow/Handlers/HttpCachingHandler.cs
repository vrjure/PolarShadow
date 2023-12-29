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
        private readonly BufferLocation _location;
        public HttpCachingHandler(IBufferCache memoryCache) :this(memoryCache, BufferLocation.Memory, new HttpClientHandler())
        {

        }

        public HttpCachingHandler(IBufferCache memoryCache, BufferLocation location) : this(memoryCache, location, new HttpClientHandler())
        {

        }

        public HttpCachingHandler(IBufferCache memoryCache, BufferLocation location, HttpMessageHandler httpMessageHandler)
        {
            _cache = memoryCache;
            InnerHandler = httpMessageHandler;
            _location = location;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var key = BufferCache.SHA(request.RequestUri.ToString());

            if (_cache.ContainsKey(key))
            {
                var content = await _cache.GetAsync(key);

                if (!_cache.ContainsKey(key, BufferLocation.Memory))
                {
                    _cache.Set(key, content, BufferLocation.Memory);
                }

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(content)
                };
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            return response;

            var data = await response.Content.ReadAsByteArrayAsync();
            await _cache.SetAsync(key, data, _location);
            return response;
        }
    }
}
