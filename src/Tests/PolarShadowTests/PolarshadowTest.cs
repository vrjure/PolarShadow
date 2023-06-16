using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Videos;

namespace PolarShadowTests
{
    internal class PolarshadowTest
    {
        [Test]
        public async Task TestSampleHtml()
        {
            var builder = new PolarShadowBuilder();
            builder.Configure(optionBuilder =>
            {
                using var fs = new FileStream("./config.json", FileMode.Open, FileAccess.Read);
                optionBuilder.ConfigureFromStream(fs);
            });

            var polarShadow = builder.Build();
            if (!polarShadow.TryGetSite("dy2018", out IPolarShadowSite site))
            {
                Assert.Fail("no site dy2018");
                return;
            }

            Console.WriteLine(await site.ExecuteAsync(VideoAbilities.Newest));
        }
    }
}
