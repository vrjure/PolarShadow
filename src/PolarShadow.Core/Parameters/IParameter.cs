using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface IParameter
    {
        bool TryGetValue(string key, out ParameterValue value);
        void Write(Utf8JsonWriter writer);
    }
}
