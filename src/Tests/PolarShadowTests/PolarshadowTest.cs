using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadowTests
{
    internal class PolarshadowTest
    {
        [Test]
        public void BuildTest()
        {
            var builder = new PolarShadowBuilder();
            builder.Configure(optionBuilder =>
            {
                using var fs = new FileStream("./config.json", FileMode.Open, FileAccess.Read);
                optionBuilder.ConfigureFromStream(fs);
            });
        }
    }
}
