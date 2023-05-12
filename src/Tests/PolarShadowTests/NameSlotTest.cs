using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace PolarShadowTests
{
    internal class NameSlotTest
    {
        private static SampleClass sample = new SampleClass()
        {
            Expensive = 10,
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
                "Test5:{$.name}",
                "Test6:{/div/a/@href:/.*/i}",
                "Test6:{//div/a/@href:/.*/i}",
                "Test7:{/div/a/@href:/.*/i} middle {right:F1} end",
                "Test8:erro format {/div/a/@href:/.*/i middle {right:F1} end",
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
             //var ns = "expensive:{expensive}, bicycle:{$..store.bicycle.price$}, ns:{$..store.bicycle.price$ - 1}{0}";
            //Console.WriteLine(ns.NameSlot(JsonDocument.Parse(JsonSerializer.Serialize(sample, JsonOption.DefaultSerializer)).RootElement));
        }
    }
}
