namespace PolarShadow.ResourcePack.Test
{
    public class Dy2018Test
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestSample()
        {
            Dy2018Site site = new Dy2018Site();
            var result = await site.GetVideoDetailAsync("https://www.dy2018.com/i/103790.html");
            Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping}));
        }
    }
}