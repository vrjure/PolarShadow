using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadowTests
{
    internal class NameSlotTest
    {
        [Test]
        public void TestFormatNameSlot()
        {
            var sample = new SampleClass()
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

            var ns = "expensive:{expensive}, bicycle:{$..store.bicycle.price$}, ns:{$..store.bicycle.price$ - 1}";
            Console.WriteLine(ns.NameSlot(JsonDocument.Parse(JsonSerializer.Serialize(sample, JsonOption.DefaultSerializer)).RootElement));
        }
    }
}
