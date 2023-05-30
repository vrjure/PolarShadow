using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadowTests
{
    internal class PolarshadowBuilderTest
    {
        [Test]
        public void BuildTest()
        {
            var builder = new PolarShadowBuilder();
            builder.Configure(optionBuilder =>
            {
                
                optionBuilder.ConfigureFromStream(new MemoryStream());
            });
        }
    }
}
