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

        private void Read(string jsonPath)
        {
            Console.WriteLine(jsonPath);
            using var sr = new StringReader(jsonPath);
            var reader = new JsonPathReader(Encoding.UTF8.GetBytes(jsonPath));
            while (reader.Read())
            {
                Console.WriteLine(reader.TokenType.ToString());
                switch (reader.TokenType)
                {
                    case JsonPathTokenType.PropertyName:
                        if (reader.TryReadProperty(out string pro))
                        {
                            Console.WriteLine("  --  "+pro);
                        }
                        break;
                    case JsonPathTokenType.Number:
                        if (reader.TryReadNumber(out int num))
                        {
                            Console.WriteLine("  --  " + num);
                        }
                        break;
                    case JsonPathTokenType.Operator:
                        if (reader.TryReadOperator(out JsonPathExpressionOperator op))
                        {
                            Console.WriteLine("  --  " +op.ToString());
                        }
                        break;
                    case JsonPathTokenType.SliceRange:
                        if (reader.TryReadSliceRange(out JsonPathSliceRange range))
                        {
                            Console.WriteLine("  --  " + range.ToString());
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
                        new Book { Category = "fiction", Author = "Evelyn waugh", Title = "Sword of Honour", Price =12.99}

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
                "store"
            };
            foreach (var item in paths)
            {
                Console.WriteLine(item);
                var isSuccess = JsonPath.TryGetPropertyWithJsonPath(doc.RootElement, item, out JsonElement result);
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
