using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface IPolarShadowItem
    {
        string Name { get; }
        void Write(Utf8JsonWriter writer);
    }
}
