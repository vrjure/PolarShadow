using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal class BasePropertyContentWriting : ContentWriting, ICloneable
    {
        private static string sitePro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.Site));
        private static string fromRequestPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.FromRequest));
        private static string imageSrcHeadersPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.ImageSrcHeaders));
        private static string imageSrcPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.ImageSrc));

        private HashSet<string> properties = new HashSet<string>();

        public override void AfterWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            if (property.NameEquals(imageSrcPro) || property.NameEquals(imageSrcHeadersPro))
            {
                properties.Add(imageSrcPro);
            }
        }

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

            if(properties.Contains(imageSrcPro) && !properties.Contains(imageSrcHeadersPro))
            {
                if (parameter.TryReadValue(imageSrcHeadersPro, out JsonElement imageSrcHeaders))
                {
                    writer.WritePropertyName(imageSrcHeadersPro);
                    writer.WriteRawValue(imageSrcHeaders.GetRawText());
                }
            }
        }

        public object Clone()
        {
            return new BasePropertyContentWriting();
        }
    }
}
