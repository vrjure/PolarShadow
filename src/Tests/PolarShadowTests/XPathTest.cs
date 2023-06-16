using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using HtmlAgilityPack;
using PolarShadow.Core;

namespace PolarShadowTests
{
    internal class XPathTest
    {
        [Test]
        public void Test()
        {
            var doc = new XPathDocument(@"./Books.Xml");

            var nav = doc.CreateNavigator();

            var iterator = nav.Select("//book");

            while (iterator != null && iterator.MoveNext())
            {
                Console.WriteLine(iterator?.Current?.Select(".@genre").Current?.Value);
            }
        }

        [Test]
        public void HtmlTest()
        {
            var doc = new XPathDocument(@"./Books.Xml");
            var element = new HtmlElement(doc.CreateNavigator());
            var select = element.Select("//book[@genre='novel']/title");
            if (select.ValueKind == HtmlValueKind.Node)
            {
                Console.WriteLine(select.GetValue());
            }
            else if (select.ValueKind == HtmlValueKind.Nodes)
            {
                foreach (var item in select.EnumerateElements())
                {
                    Console.WriteLine(item.GetValue());
                }
            }
        }

        [Test]
        public void HtmlAgilityPackXpathTest()
        {
            using var fs = new FileStream(@"C:\Users\vrjure\Desktop\error_gb2312.html", FileMode.Open, FileAccess.Read);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var doc = new HtmlDocument();
            doc.Load(fs, Encoding.GetEncoding("gb2312"));

            //using var ms = new MemoryStream();
            //doc.Save(ms);
            //var xpathDoc = new XPathDocument(XmlReader.Create(ms, new XmlReaderSettings
            //{
            //    DtdProcessing = DtdProcessing.Ignore,
            //    ConformanceLevel = ConformanceLevel.Fragment
            //}));
            //var result = xpathDoc.CreateNavigator().Select("//body/div[@id='header']/div[@class='contain']/div[@class='bd2']/div[@class='bd3']/div[@class='bd3r']/div[@class='co_area2']/div[@class='co_content8']/ul/table");
            var result = doc.CreateNavigator().Select("//body/div[@id='header']/div[@class='contain']/div[@class='bd2']/div[@class='bd3']/div[@class='bd3r']/div[@class='co_area2']/div[@class='co_content8']/ul//table");
            Console.WriteLine($"count:{result.Count}");
            while (result!.MoveNext())
            {
                var child = result!.Current!.Select(".//a/@title");
                
                while (child!.MoveNext())
                {
                    Console.WriteLine(child!.Current!.Value);
                }
            }
        }
    }
}
