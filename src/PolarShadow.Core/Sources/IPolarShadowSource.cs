using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PolarShadow.Core
{
    public interface IPolarShadowSource
    {
        void Save(Stream content);
        IPolarShadowProvider Build(IPolarShadowBuilder builder);
    }
}
