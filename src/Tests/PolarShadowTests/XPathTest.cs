using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace PolarShadowTests
{
    internal class XPathTest
    {
        [Test]
        public void Test()
        {
            var doc  = new XPathDocument(@"C:\Users\vrjure\Desktop\Books.Xml");
            var nav = doc.CreateNavigator();
            var iterator = nav.Select("//book[1]/@genre");
            
            while (iterator!= null && iterator.MoveNext())
            {
                Console.WriteLine(iterator?.Current?.Value);
            }
        }
    }
}
