using PolarShadow.Core;
using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal class DetailContentWriting : ContentWriting, ICloneable
    {
        private HashSet<string> writedPro = new HashSet<string>();

        private static string namePro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.Name));
        private static string imageSrcPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.ImageSrc));
        private static string descPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.Description));
        private static string srcPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.Src));
        private static string imageSrcHeadersPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.ImageSrcHeaders));

        public override void AfterWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            writedPro.Add(property.Name);
        }

        public override void BeforeWriteStartArray(Utf8JsonWriter writer, string propertyName, IParameter parameter)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (propertyName.Equals(nameof(ResourceTree.Children), StringComparison.OrdinalIgnoreCase))
            {
                if (!writedPro.Contains(namePro) && parameter.TryReadValue("$.name", out string name))
                {
                    writer.WriteString(namePro, name);
                }

                if (!writedPro.Contains(imageSrcPro) && parameter.TryReadValue("$.imageSrc", out string imageSrc))
                {
                    writer.WriteString(imageSrcPro, imageSrc);
                }

                if (!writedPro.Contains(imageSrcHeadersPro) && parameter.TryReadValue("$.imageSrcHeaders", out JsonElement imageSrcHeaders))
                {
                    writer.WritePropertyName(imageSrcHeadersPro);
                    writer.WriteRawValue(imageSrcHeaders.GetRawText());
                }

                if (!writedPro.Contains(descPro) && parameter.TryReadValue("$.description", out string desc))
                {
                    writer.WriteString(descPro, desc);
                }

                if (!writedPro.Contains(srcPro) && parameter.TryReadValue("$.src", out string src))
                {
                    writer.WriteString(srcPro, src);
                }

                writedPro.Add(namePro);
                writedPro.Add(imageSrcPro);
                writedPro.Add(descPro);
                writedPro.Add(srcPro);
                writedPro.Add(imageSrcHeadersPro);
            }
            else
            {
                writedPro.Add(propertyName);
            }
        }

        public object Clone()
        {
            return new DetailContentWriting();
        }
    }
}
