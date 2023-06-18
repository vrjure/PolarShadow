using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    internal class HttpResource : IHttpResource
    {
        protected HttpClient _client;
        protected IJSRuntime _js;
        public HttpResource(IHttpClientFactory factory, IJSRuntime js)
        {
            _client = factory.CreateClient("default");
            _js = js;
        }
        public virtual async Task<string> CreateObjectUrlAsync(string orign)
        {
            var stream = await _client.GetStreamAsync(orign);
            return await _js.CreateObjectUrl(stream);
        }

        public virtual async Task RevokeObjectUrlAsync(string objectUrl)
        {
            await _js.RevokeObjectUrlAsync(objectUrl);
        }
    }
}
