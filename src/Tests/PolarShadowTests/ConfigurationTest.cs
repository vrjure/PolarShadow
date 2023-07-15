using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PolarShadow.Core.Configurations;

namespace PolarShadowTests
{
    
    public class ConfigurationTest
    {
        [Test]
        public void ConfigurationNormalTest()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("./config.json", false, true);
            var config = builder.Build();
            var provider = config.Providers.First();
            var keys = provider.GetChildKeys(Enumerable.Empty<string>(), default).Distinct(StringComparer.OrdinalIgnoreCase);
            foreach (var key in keys)
            {
                Console.WriteLine(key);
            }
        }

        [Test]
        public void ConfigurationJsonWriterTest()
        {
            var builder = new ConfigurationBuilder();
            builder.AddPolarShadowJsonFile("./config.json", false, false);
            builder.AddJsonFile("./source.json", false, false);
            var config = builder.Build();
            using var ms = new MemoryStream();
            config.WritePolarShadowConfigurationToStream(ms, new JsonWriterOptions { Indented = true });
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);
            Console.WriteLine(sr.ReadToEnd());
        }

        private IEnumerable<IConfigurationSection> EnumerablePath(IConfiguration config)
        {
            foreach (var item in config.GetChildren())
            {
                yield return item;
                if (item.Value == null)
                {
                    foreach (var child in EnumerablePath(item))
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}
