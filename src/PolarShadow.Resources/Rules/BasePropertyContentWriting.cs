using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal class BasePropertyContentWriting : ContentWriting
    {
        private static string sitePro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.Site));
        private static string fromRequestPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.FromRequest));

        public override void BeforeWriteEndObject(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (parameter.TryReadValue(SiteParams.SiteName, out string siteName))
            {
                writer.WriteString(sitePro, siteName);
            }

            if (parameter.TryReadValue(SiteParams.SiteRequest, out string requestName))
            {
                writer.WriteString(fromRequestPro, requestName);
            }
        }
    }
}
