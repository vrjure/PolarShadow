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
        public void TestOther()
        {

        }
    }
}