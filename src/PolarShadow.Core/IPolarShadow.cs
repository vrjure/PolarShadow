using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPolarShadow
    {
        IEnumerable<IPolarShadowItem> Items { get; }
        void Load(IPolarShadowSource source, bool reLoad = false);
        void WriteTo(Utf8JsonWriter writer);
    }
}
