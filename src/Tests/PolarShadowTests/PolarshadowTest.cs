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
            builder.ConfigureDefault()
                .AddJsonFileSource(Path.Combine(Directory.GetCurrentDirectory(), "Config.json"))
                .AddJsonFileSource(Path.Combine(Directory.GetCurrentDirectory(), "Config2.json"));

            var polarShadow = builder.Build();

            Print(polarShadow);
        }

        [Test]
        public void BuildAndChangeTest()
        {
            var builder = new PolarShadowBuilder();
            builder.ConfigureDefault()
                .AddJsonFileSource(Path.Combine(Directory.GetCurrentDirectory(), "Config.json"))
                .AddJsonFileSource(Path.Combine(Directory.GetCurrentDirectory(), "Config2.json"));

            var polarShadow = builder.Build();
            polarShadow.
        }

        private void Print(IPolarShadow ps)
        {
            using var ms = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true });
            ps.Write(jsonWriter);
            jsonWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);
            Console.WriteLine(sr.ReadToEnd());
        }
    }
}
