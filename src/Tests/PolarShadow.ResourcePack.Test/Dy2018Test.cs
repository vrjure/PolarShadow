using System.Text;

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
            var result = await site.GetVideoDetailAsync(new VideoSummary { DetailSrc = "https://www.dy2018.com/i/103790.html" });
            Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping}));
        }

        [Test]
        public async Task TestSearch()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6,zh-TW;q=0.5");
            client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("HOST", "www.dy2018.com");
            client.DefaultRequestHeaders.Add("Origin", "https://www.dy2018.com");
            client.DefaultRequestHeaders.Add("referer", "https://www.dy2018.com/index.html");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"107\", \"Chromium\";v=\"107\", \"Not=A?Brand\";v=\"24\"");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "Windows");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62");
            //client.DefaultRequestHeaders.Add("Connection", "close");

            var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("show", "title%2Csmalltext"),
                new KeyValuePair<string, string>("tempid", "1"),
                new KeyValuePair<string, string>("keyboard", "%CB%C0%C9%F1"),
                new KeyValuePair<string, string>("Submit", "%C1%A2%BC%B4%CB%D1%CB%F7")
            });

            var result = await client.PostAsync("https://www.dy2018.com/e/search/index.php", content);
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.Redirect);
            Console.WriteLine(await result?.Content?.ReadAsStringAsync());
        }
    }
}