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

        public override bool BeforeWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            if (property.Name.Equals(nameof(ResourceTree.Children), StringComparison.OrdinalIgnoreCase))
            {
                if (!writedPro.Contains(namePro) && parameter.TryReadValue("$.name", out string name))
                {
                    writer.WriteString(namePro, name);
                    writedPro.Add(namePro);
                }

                if (!writedPro.Contains(imageSrcPro) && parameter.TryReadValue("$.imageSrc", out string imageSrc))
                {
                    writer.WriteString(imageSrcPro, imageSrc);
                    writedPro.Add(imageSrcPro);
                }

                if (!writedPro.Contains(descPro) && parameter.TryReadValue("$.description", out string desc))
                {
                    writer.WriteString(descPro, desc);
                    writedPro.Add(descPro);
                }
            }
            else
            {
                writedPro.Add(property.Name);
            }

            return false;
        }

        public override void AfterWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            base.AfterWriteProperty(writer, property, parameter);
        }

        public object Clone()
        {
            return new DetailContentWriting();
        }
    }
}
