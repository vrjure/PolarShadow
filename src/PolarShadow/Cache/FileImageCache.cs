using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarShadow;

namespace PolarShadow.Cache
{
    internal class FileImageCache : IImageCache
    {
        private readonly IJSRuntime _js;
        private HttpClient _http;
        public FileImageCache(IJSRuntime js, HttpClient http) 
        {
            _js = js;
            _http = http;
        }
        public async Task<string> GetCacheUrlAsync(string imageSrc)
        {
            if (string.IsNullOrEmpty(imageSrc))
            {
                return imageSrc;
            }
            var hash = imageSrc.Hash();
            if (!hash.IsImageCached())
            {
                var stream = await _http.GetStreamAsync(imageSrc);
                await hash.CacheImageAsync(stream);
            }

            using var fs = hash.ReadStream();
            var dotnetStream = new DotNetStreamReference(fs);
            return await _js.InvokeAsync<string>("createObjectUrl", dotnetStream);
        }

        public async Task RevokeUrlAsync(string url)
        {
            await _js.InvokeVoidAsync("revokeObjectUrl", url);
        }
    }
}
