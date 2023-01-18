using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
