using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Xml.XPath;

namespace PolarShadowTests
{
    internal class NameSlotTest
    {
        private static SampleClass sample = new SampleClass()
        {
            Expensive = 11,
            Store = new Store
            {
                Book = new Book[]
                    {
                        new Book { Category = "reference", Author = "Nigel Rees", Title="Sayings of the Century", Price = 8.98},
                        new Book { Category = "fiction", Author = "Evelyn waugh", Title = "Sword of Honour", Price =12.99}

                    },
                Bicycle = new Bicycle
                {
                    Color = "red",
                    Price = 19.95
                }
            }
        };

        [Test]
        public void NameSlotReaderTest()
        {
           var readTestList = new[]
            {
                "Test1:{name} end",
                "Test2:{name} {title} end",
                "Test3:{name: N2}",
                "Test4:{name:/.*/}",
                "Test5:{name:/.*/i}",
                "Test6:{$.name}",
                "Test7:{///div/a[@href='http']/@href}",
                "Test8:{/div/a/@href:/.*/i}",
                "Test9:{//div/a/@href:/.*/i}",
                "Test10:{/div/a/@href:/.*/i} middle {right:F1} end",
                "Test11:{///div/a/@href:/.*/i} middle {right:F1} end",
                "Test12:erro format {/div/a/@href:/.*/i middle {right:F1} end",
                "Test12:condition expression: {/div/a/@href:/.*/i ? 'a': 'b'} end",
                "Test12:condition expression: {/div/a/@href:/.*/i == 'abc' ? 'a': 'b'} end",
                "Test12:error condition expression: {/div/a/@href:/.*/i == 'abc' : 'a': 'b'} end",
                "Test12:raw: {{/div/a/@href:/.*/i == 'abc' ? 'a': '{b}'}} end",
            };

            foreach (var item in readTestList)
            {
                Console.Write(">>> ");
                Console.WriteLine(item);
                var reader = new NameSlotReader(item);
                while (reader.Read())
                {
                    Console.Write(reader.TokenType);
                    Console.Write(":");
                    Console.WriteLine(reader.GetString());
                }

                Console.WriteLine("------------------------------------------");
            }
        }

        [Test]
        public void TestFormatNameSlot()
        {

            var keyValues = new KeyValueParameter();
            keyValues.Add("page", 1);
            keyValues.Add("title", "good");

            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(sample, JsonOption.DefaultSerializer));
            var objectValues = new ObjectParameter();
            var xpathDoc = new XPathDocument("./Books.Xml");
            objectValues.Add(new HtmlElement(xpathDoc));
            objectValues.Add(doc.RootElement.Clone());
            var values = new Parameters(keyValues, objectValues);
            var list = new string[]
            {
                "get page: {page}",
                "get title:{title}",
                "get json value:{$.expensive}",
                "get json value:{$.expensive:N5}",
                "get json value:{$.expensive:R}",
                "compare json value:{$.expensive:R == '11' ? 'true' : 'false'}",
                "get html value:{///book[@genre='novel']/title}",
                "get html value:{///book[@genre='novel']/title:/.*fid/}",
                "get html value:{///book[@genre='novel']/title:/.*fid/i}",
                "compare html value:{///book[@genre='novel']/title:/.*fid/i=='The Confid' ? '{title}':'false'}"
            };

            foreach (var item in list)
            {
                Console.WriteLine($">>> {item}");
                Console.WriteLine(item.Format(values));
                Console.WriteLine("-----------------------------------");
            }
        }

        [Test]
        public void JsonTextNameSlotTest()
        {
            using var fs = new FileStream("./source.json", FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            var reader = new NameSlotReader(sr.ReadToEnd());
            while (reader.Read())
            {
                Console.Write(reader.TokenType);
                Console.Write(":");
                Console.WriteLine(reader.GetString());
            }
        }
    }
}
