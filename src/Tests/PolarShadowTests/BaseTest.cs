using PolarShadow.Core;
using System.Text.Json;

namespace PolarShadowTests
{
    public class BaseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public static void TestBuilder()
        {
            PolarShadowBuilder builder = new PolarShadowBuilder();
            builder.AutoSite(typeof(Dy2018Site).Assembly);
            var ps = builder.Build();

            var site = ps.GetSite("NEWZMZ");
            Assert.IsNotNull(site);
            Assert.IsTrue(site.Name == "NEWZMZ");

        }

        [Test]
        public async Task TestFull()
        {
            PolarShadowBuilder builder = new PolarShadowBuilder();
            builder.AutoSite(typeof(Dy2018Site).Assembly);
            var ps = builder.Build();

            var searcHandler = ps.BuildSearchHandler(new SearchVideoFilter(1, 10, "À¿"));
            var result = await searcHandler.SearchNextAsync();
            while (result != null)
            {
                Console.WriteLine(JsonSerializer.Serialize(result, JsonOption.DefaultSerializer));
                result = await searcHandler.SearchNextAsync();
            }

            Assert.Pass();
        }

        [Test]
        public async Task TestFormConfig()
        {
            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "source.json");
            using var fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            var configs = JsonSerializer.Deserialize<PolarShadowSiteConfig[]>(sr.ReadToEnd(), JsonOption.DefaultSerializer);
            PolarShadowBuilder builder = new PolarShadowBuilder(configs);
            var ps = builder.Build();
            var searcHandler = ps.BuildSearchHandler(new SearchVideoFilter(1, 10, "À¿…Ò"));
            var result = await searcHandler.SearchNextAsync();
            while (result != null)
            {
                Console.WriteLine(JsonSerializer.Serialize(result, JsonOption.DefaultSerializer));
                result = await searcHandler.SearchNextAsync();
            }
        }
    }
}