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
            using var doc = JsonDocument.Parse(fs);
            var kvp = new KeyValueParameter();
            if (doc.RootElement.TryGetProperty("parameters", out JsonElement parameters))
            {
                kvp.Add(parameters);
            }

            var resuest = doc.RootElement.GetProperty("request");
            if (resuest.TryGetProperty("parameters", out JsonElement requestPara))
            {
                kvp.Add(requestPara);
            }

            Console.WriteLine("request:");
            Console.WriteLine(resuest.GetRawText().Format(kvp));

            var p = new Parameters();
            p.Add(kvp);
            using var fsxml = new FileStream("./Books.xml", FileMode.Open, FileAccess.Read);
            var xmldoc = new XPathDocument(fsxml);
            var op = new ObjectParameter();
            op.Add(xmldoc);
            p.Add(op);

            var response = doc.RootElement.GetProperty("response");
            Console.WriteLine("response:");
            Console.WriteLine(response.GetRawText().Format(p));
        }

        [Test]
        public void TestAnalysis()
        {
            using var fs = new FileStream("./source.json", FileMode.Open, FileAccess.Read);
            using var doc = JsonDocument.Parse(fs);

            var kvp = new KeyValueParameter();
            if (doc.RootElement.TryGetProperty("parameters", out JsonElement parameters))
            {
                kvp.Add(parameters);
            }

            var request = doc.RootElement.GetProperty("request");
            if (request.TryGetProperty("parameters", out JsonElement requestPara))
            {
                kvp.Add(requestPara);
            }

            var body = request.GetProperty("body");

            Console.WriteLine("body:");
            using var ms = new MemoryStream();
            new ContentWriter().Build(ms, body, kvp);
            using var sr = new StreamReader(ms);
            Console.WriteLine(sr.ReadToEnd());

            using var fsxml = new FileStream("./Books.xml", FileMode.Open, FileAccess.Read);
            var xmldoc = new XPathDocument(fsxml);

            var content = new ObjectParameter();
            content.Add(new HtmlElement(xmldoc));

            var ps = new Parameters();
            ps.Add(kvp);
            ps.Add(content);
            var response = doc.RootElement.GetProperty("response2");
            var responseContent = response.GetProperty("content");
            using var ms2 = new MemoryStream();
            new ContentWriter().Build(ms2, responseContent, ps);
            using var sr2 = new StreamReader(ms2);
            Console.WriteLine("response2:");
            Console.WriteLine(sr2.ReadToEnd());
        }
    }
}
