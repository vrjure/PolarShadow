using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace PolarShadowTests
{
    public class AnalysisTest
    {
        [Test]
        public void TestAnalysisText()
        {
            using var fs = new FileStream("./source.json", FileMode.Open, FileAccess.Read);
            var doc = JsonDocument.Parse(fs);
            var input = new NameSlotValueCollection();
            if (doc.RootElement.TryGetProperty("parameters", out JsonElement parameters))
            {
                input.AddNameValue(parameters);
            }

            var resuest = doc.RootElement.GetProperty("request");
            if (resuest.TryGetProperty("parameters", out JsonElement requestPara))
            {
                foreach (var item in requestPara.EnumerateArray())
                {
                    input.Add(item);
                }
            }

            Console.WriteLine("request:");
            Console.WriteLine(resuest.GetRawText().Format(input));

            using var fsxml = new FileStream("./Books.xml", FileMode.Open, FileAccess.Read);
            var xmldoc = new XPathDocument(fsxml);
            input.Add(new HtmlElement(xmldoc));

            var response = doc.RootElement.GetProperty("response");
            Console.WriteLine("response:");
            Console.WriteLine(response.GetRawText().Format(input));
        }

        [Test]
        public void TestAnalysis()
        {
            using var fs = new FileStream("./source.json", FileMode.Open, FileAccess.Read);
            var doc = JsonDocument.Parse(fs);
            var input = new NameSlotValueCollection();
            if (doc.RootElement.TryGetProperty("parameters", out JsonElement parameters))
            {
                input.AddNameValue(parameters);
            }

            var request = doc.RootElement.GetProperty("request");
            if (request.TryGetProperty("parameters", out JsonElement requestPara))
            {
                foreach (var item in requestPara.EnumerateArray())
                {
                    input.Add(item);
                }
            }

            var body = request.GetProperty("body");

            Console.WriteLine("body:");
            using var ms = new MemoryStream();
            body.BuildContent(ms, input);
            using var sr = new StreamReader(ms);
            Console.WriteLine(sr.ReadToEnd());

            using var fsxml = new FileStream("./Books.xml", FileMode.Open, FileAccess.Read);
            var xmldoc = new XPathDocument(fsxml);
            input.Add(new HtmlElement(xmldoc));

            var response = doc.RootElement.GetProperty("response2");
            var responseContent = response.GetProperty("content");
            using var ms2 = new MemoryStream();
            responseContent.BuildContent(ms2, input);
            using var sr2 = new StreamReader(ms2);
            Console.WriteLine("response2:");
            Console.WriteLine(sr2.ReadToEnd());
        }
    }
}
