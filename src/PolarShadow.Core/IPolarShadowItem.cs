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
        void Load(IPolarShadowProvider provider);
        void WriteTo(Utf8JsonWriter writer);
    }
}
