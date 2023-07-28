using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Videos
{
    internal class VideoSummaryContentWriting : ContentWriting
    {
        public override string[] RequestFilter => new string[] { VideoRequests.Search, VideoRequests.Newest };
        public override void AfterWriteProperty(Utf8JsonWriter writer, JsonProperty property, IParameter parameter)
        {
            if (property.Name.Equals(nameof(VideoSummary.DetailSrc), StringComparison.OrdinalIgnoreCase))
            {
                if (parameter.TryReadValue("site:name", out string siteName))
                {
                    writer.WriteString("siteName", siteName);
                }
            }
        }
    }
}
