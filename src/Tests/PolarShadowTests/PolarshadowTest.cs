using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PolarShadow.Core;
using PolarShadow.Videos;

namespace PolarShadowTests
{
    public class PolarshadowTest
    {
        [Test]
        public void BuildTest()
        {
            var builder = new PolarShadowBuilder();
            builder.ConfigureDefault();
            var polarShadow = builder.Build();

            polarShadow.LoadJsonFileSource("./config.json");
            polarShadow.LoadJsonFileSource("./config2.json");

            Print(polarShadow);
        }

        private void Print(IPolarShadow ps)
        {
            using var ms = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true, Encoder = JsonOption.DefaultSerializer.Encoder });
            ps.WriteTo(jsonWriter);
            jsonWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);
            Console.WriteLine(sr.ReadToEnd());
        }
    }
}
