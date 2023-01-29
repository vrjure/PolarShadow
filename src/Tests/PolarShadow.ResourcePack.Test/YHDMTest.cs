using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ResourcePack.Test
{
    public class YHDMTest
    {
        [Test]
        public async Task TestSampe()
        {
            var site = new YHDMSite();
            var summay = await site.SearchVideosAsync(new Core.SearchVideoFilter(1, 10, "死神"));
            Console.WriteLine(JsonSerializer.Serialize(summay, JsonOption.DefaultSerializer));

            var detail = await site.GetVideoDetailAsync(summay.Data.First());
            Console.WriteLine(JsonSerializer.Serialize(detail, JsonOption.DefaultSerializer));
        }
    }
}
