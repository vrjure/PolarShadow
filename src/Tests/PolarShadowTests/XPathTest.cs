using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using PolarShadow.Core;

namespace PolarShadowTests
{
    internal class XPathTest
    {
        [Test]
        public void Test()
        {
            var doc  = new XPathDocument(@"C:\Users\vrjure\Desktop\Books.Xml");
       
            var nav = doc.CreateNavigator();

            var iterator = nav.Select("//book");
            
            while (iterator!= null && iterator.MoveNext())
            {
                Console.WriteLine(iterator?.Current?.Select(".@genre").Current?.Value);
            }
        }

        [Test]
        public void HtmlTest()
        {
            var doc = new XPathDocument(@"C:\Users\vrjure\Desktop\Books.Xml");
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
    }
}
