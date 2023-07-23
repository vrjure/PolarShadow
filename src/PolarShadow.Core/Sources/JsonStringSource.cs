using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PolarShadow.Core
{
    public class JsonStringSource : JsonSource
    {
        public string Json { get; set; }
        public override IPolarShadowProvider Build(IPolarShadowBuilder builder)
        {
            return new JsonStringProvider(this);
        }

        public override void Save(Stream content)
        {
            using var sr = new StreamReader(content);
            Json = sr.ReadToEnd();
        }
    }
}
