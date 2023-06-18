using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarShadow;

namespace PolarShadow.Cache
{
    internal class HttpFileResource : HttpResource, IHttpFileResource
    {
        public HttpFileResource(IHttpClientFactory factory, IJSRuntime js) :base(factory, js)
        {
        }
        public override async Task<string> CreateObjectUrlAsync(string imageSrc)
        {
            if (string.IsNullOrEmpty(imageSrc))
            {
                return imageSrc;
            }
            var hash = imageSrc.Hash();
            if (!hash.IsImageCached())
            {
                var stream = await _client.GetStreamAsync(imageSrc);
                await hash.CacheImageAsync(stream);
            }

            using var fs = hash.ReadStream();
            return await _js.CreateObjectUrl(fs);
        }

        public void RemoveCache(string imageSrc)
        {
            if (string.IsNullOrEmpty(imageSrc))
            {
                return;
            }

            var hash = imageSrc.Hash();
            hash.RemoveImageCache();
        }
    }
}
