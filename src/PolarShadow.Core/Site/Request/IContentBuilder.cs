using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public interface IContentBuilder
    {
        void BuildContent(Utf8JsonWriter writer, JsonElement template, IParameter parameter);
    }
}
