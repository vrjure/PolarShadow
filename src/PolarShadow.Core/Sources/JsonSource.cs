using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PolarShadow.Core
{
    public abstract class JsonSource : IPolarShadowSource
    {
        public abstract IPolarShadowProvider Build(IPolarShadowBuilder builder);

        public abstract void Save(Stream content);
    }
}
