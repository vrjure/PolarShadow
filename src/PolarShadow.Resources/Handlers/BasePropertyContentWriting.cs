using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal class BasePropertyContentWriting : ContentWriting
    {
        public override string[] RequestFilter => new string[] { "*" };

        public override void AfterWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            if (property.Name.Equals(nameof(Link.Src), StringComparison.OrdinalIgnoreCase))
            {
                if (parameter.TryReadValue("site:name", out string siteName))
                {
                    writer.WriteString("site", siteName);
                }
                if (parameter.TryReadValue("site:request", out string requestName))
                {
                    writer.WriteString("fromRequest", requestName);
                }
            }
        }
    }
}
