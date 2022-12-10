using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PolarShadow.Core.Aria2RPC;

namespace PolarShadowTests
{
    public class JsonTest
    {
        [Test]
        public void TestJsonDash()
        {
            var origin = new DashClass
            {
                Dash_Name = "d"
            };
            var json = JsonSerializer.Serialize(origin, JsonOption.ForDash);

            Console.WriteLine(json);

            var opt = JsonSerializer.Deserialize<DashClass>(json, JsonOption.ForDash);
            Console.WriteLine(opt.Dash_Name);
            Assert.That(opt?.Dash_Name, Is.EqualTo(origin.Dash_Name));
        }
    }

    class DashClass
    {
        public string? Dash_Name { get; set; }
    }
}
