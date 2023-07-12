using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IPolarShadowSource
    {
        IPolarShadowProvider Build(IPolarShadowBuilder builder);
    }
}
