using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public abstract class FileSource : IPolarShadowSource
    {
        public string Path { get; set; }
        public abstract IPolarShadowProvider Build(IPolarShadowBuilder builder);
    }
}
