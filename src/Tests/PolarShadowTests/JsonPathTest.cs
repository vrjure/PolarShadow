using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadowTests
{
    public class JsonPathTest
    {
        private string[] _jsonPath = new[]
        {
            "$.store.book[*].author",
            "$.store.book['aaa','bbb'].author",
            "$..author",
            "$.store.*",
            "$.store..price",
            "$..book[2]",
            "$..book[-2]",
            "$..book[0,1]",
            "$..book[:2]",
            "$..book[1:2]",
            "$..book[-2:]",
            "$..book[2:]",
            "$..book[?(@.isbn)]",
            "$.store.book[?(@.price < 10)]",
            "$..book[?(@.price <= $['expensive'])]",
            "$..*"
        };

        [Test]
        public void JsonPathReaderTest()
        {
            foreach (var item in _jsonPath)
            {
                Read(item);
            }
        }

        [Test]
        public void JsonPathReaderToEndTest()
        {
            foreach (var item in _jsonPath)
            {
                var reader = new JsonPathReader(Encoding.UTF8.GetBytes(item));
                var consume = reader.ReadToEnd();
                Console.WriteLine($"total:{item.Length}, consume ：{consume}");
            }
        }

        private void Read(string jsonPath)
        {
            Console.WriteLine(jsonPath);
            var reader = new JsonPathReader(Encoding.UTF8.GetBytes(jsonPath));
            while (reader.Read())
            {
                Console.WriteLine(reader.TokenType.ToString());
                switch (reader.TokenType)
                {
                    case JsonPathTokenType.PropertyName:
                        if (reader.TryGetString(out string pro))
                        {
                            Console.WriteLine("  --  "+pro);
                        }
                        break;
                    case JsonPathTokenType.Number:
                        if (reader.TryGetDecimal(out decimal num))
                        {
                            Console.WriteLine("  --  " + num);
                        }
                        break;
                    case JsonPathTokenType.String:
                        if (reader.TryGetString(out string s))
                        {
                            Console.WriteLine(" -- " + s);
                        }
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine("-------------------------------");
        }

        [Test]
        public void TestJsonPath()
        {
            var sample = new SampleClass()
            {
                Expensive = 10,
                Store = new Store
                {
                    Book = new Book[]
                    {
                        new Book { Category = "reference", Author = "Nigel Rees", Title="Sayings of the Century", Price = 8.98},
                        new Book { Category = "fiction", Author = "Evelyn waugh", Title = "Sword of Honour", Price =12.99, Bicycles = new Bicycle[]
                            {
                                new Bicycle{ Color = "red", Price = 1}
                            }
                        }

                    },
                    Bicycle = new Bicycle
                    {
                        Color = "red",
                        Price = 19.95
                    }
                }
            };
            var doc = JsonDocument.Parse(JsonSerializer.Serialize(sample, JsonOption.DefaultSerializer));

            var paths = new string[]
            {
                "$.expensive",
                "$..store",
                "$..book",
                "$..bicycle",
                "store",
                "$..book[1]",
                "$..book[1].bicycles[0]"
            };
            foreach (var item in paths)
            {
                Console.WriteLine(item);
                var isSuccess = JsonHelper.TryGetPropertyWithJsonPath(doc.RootElement, item, out JsonElement result);
                if (isSuccess)
                {
                    switch (result.ValueKind)
                    {
                        case JsonValueKind.Undefined:
                            break;
                        case JsonValueKind.Object:
                            Console.WriteLine(string.Join(",", result.EnumerateObject().Select(f => f.Name)));
                            break;
                        case JsonValueKind.Array:
                            Console.WriteLine(result.GetArrayLength());
                            break;
                        case JsonValueKind.String:
                            Console.WriteLine(result.GetString());
                            break;
                        case JsonValueKind.Number:
                            Console.WriteLine(result.GetDecimal());
                            break;
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            Console.WriteLine(result.GetBoolean());
                            break;
                        case JsonValueKind.Null:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("get failed");
                }
            }
        }
    }

    public class Book
    {
        public string Category { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public ICollection<Bicycle> Bicycles { get; set; }
    }
    public class Bicycle
    {
        public string Color { get; set; }
        public double Price { get; set; }
    }
    public class Store
    {
        public ICollection<Book>? Book { get; set; }
        public Bicycle Bicycle { get; set; }
    }
    public class SampleClass
    {
        public Store Store { get; set; }
        public int Expensive { get; set; }
    }
}
