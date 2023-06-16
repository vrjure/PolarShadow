using PolarShadow.Core.Aria2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PolarShadow.Core;

namespace PolarShadowTests
{
    public class JsonTest
    {
        [Test]
        public void TestJsonDash()
        {
            var origin = new DashClass
            {
                Dash_Name = "d",
                A = "A"
            };
            var json = JsonSerializer.Serialize(origin, JsonOption.ForDashSerializer);

            Console.WriteLine(json);

            var opt = JsonSerializer.Deserialize<DashClass>(json, JsonOption.ForDashSerializer);
            Console.WriteLine(opt.Dash_Name);
            Assert.That(opt?.Dash_Name, Is.EqualTo(origin.Dash_Name));
        }

        [Test]
        public void TestAria2Params()
        {
            var paramaters = new List<object>();
            paramaters.Add("token:sssss");
            paramaters.Add(new string[] {"1234", "123456"});
            paramaters.Add(new DashClass { Dash_Name= "d" });
            paramaters.Add(null);
            paramaters.Add(1);

            Console.WriteLine(JsonSerializer.Serialize(paramaters, JsonOption.ForDashSerializer));
        }

        [Test]
        public void TestEnum()
        {
            var status = new Aria2Status
            {
                Status = Aria2DownloadStatus.error
            };
            Console.WriteLine(JsonSerializer.Serialize(status, JsonOption.DefaultSerializer));
        }
    }

    class DashClass
    {
        public string? Dash_Name { get; set; }
        internal string? A { get; set; }
    }

    class A
    {
        public string AA { get; set; }
    }

    class B
    {
        public string BB { get; set; }
    }

}
