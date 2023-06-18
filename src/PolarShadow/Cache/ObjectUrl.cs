using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    internal static class ObjectUrl
    {
        public static async Task<string> CreateObjectUrl(this IJSRuntime js, Stream stream)
        {
            var dotnetStream = new DotNetStreamReference(stream);
            return await js.InvokeAsync<string>("createObjectUrl", dotnetStream);
        }

        public static async Task RevokeObjectUrlAsync(this IJSRuntime js, string url)
        {
            await js.InvokeVoidAsync("revokeObjectUrl", url);
        }
    }
}
