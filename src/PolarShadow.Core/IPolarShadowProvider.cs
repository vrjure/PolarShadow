using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface IPolarShadowProvider
    {
        bool TryGet(string name, out JsonElement value);
        void Load();
    }
}
