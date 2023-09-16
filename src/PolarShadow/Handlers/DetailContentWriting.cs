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
        public override string[] RequestFilter => new[] { Requests.Detail };

        private HashSet<string> writedPro = new HashSet<string>();

        private static string namePro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.Name));
        private static string imageSrcPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.ImageSrc));
        private static string descPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.Description));
        private static string srcPro = JsonNamingPolicy.CamelCase.ConvertName(nameof(Resource.Src));

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
