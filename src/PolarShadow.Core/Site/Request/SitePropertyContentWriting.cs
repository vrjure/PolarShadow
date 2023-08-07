using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class SitePropertyContentWriting : ContentWriting
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
            }
        }
    }
}
