using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PolarShadow.Videos;

namespace PolarShadowTests
{
    internal class PolarshadowTest
    {
        [Test]
        public void TestOptionBuilder()
        {
            var builder = new PolarShadowBuilder();
            builder.Configure(optionBuilder =>
            {
                using var fs = new FileStream("./config.json", FileMode.Open, FileAccess.Read);
                optionBuilder.Load(fs);

                //optionBuilder.AddWebAnalysisSite(new WebAnalysisSource { Name = "test", Src = "testsrc" });
                //optionBuilder.AddParameter("test1", "test2");
                //optionBuilder.GetSite("testSite")
                //.AddParameter("test", "test")
                //.AddAbility("testability")
                //.AddParameter("test", "test")
                //.SetRequest(new AnalysisRequest { Url = "www.example.com" })
                //.SetResponse(new AnalysisResponse
                //{
                //    Encoding = "gb2312"
                //});
            });

            var ps = builder.Build();
            ps.Builder.Configure(op =>
            {
                //op.RemoveSite("dy2018");
                using var ms = new MemoryStream();
                op.WriteTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                using var sr = new StreamReader(ms);
                Console.WriteLine(sr.ReadToEnd());
            });
        }

        [Test]
        public async Task TestSampleHtml()
        {
            var builder = new PolarShadowBuilder();
            builder.Configure(optionBuilder =>
            {
                using var fs = new FileStream("./config.json", FileMode.Open, FileAccess.Read);
                optionBuilder.Load(fs);
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
