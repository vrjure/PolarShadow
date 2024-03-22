using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    public interface ISiteRequest : IRequest
    {
        IKeyValueParameter Parameters { get; }
    }
}
