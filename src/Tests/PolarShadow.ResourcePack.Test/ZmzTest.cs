using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ResourcePack.Test
{
    internal class ZmzTest
    {
        [Test]
        public async Task TestSample()
        {
            NewZMZSite site = new NewZMZSite();
            var result = await site.SearchVideosAsync(new SearchVideoFilter(1, 10, "龙"));

            Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

            var summary = result.Data.Skip(1).FirstOrDefault();
            var detail = await site.GetVideoDetailAsync(summary?.DetailSrc, summary);
            Console.WriteLine(JsonSerializer.Serialize(detail, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        }
    }
}
