using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Videos
{
    internal class VideoDiscoverHandler : SequentialRequestBase<ICollection<VideoSummary>>
    {
        public VideoDiscoverHandler(string requestName, IEnumerable<ISite> sites) : base(requestName, sites)
        {
        }

        protected override bool HasResult(Stream stream)
        {
            if (stream.Length == 0)
            {
                return false;
            }

            var data = JsonSerializer.Deserialize<ICollection<VideoSummary>>(stream, JsonOption.DefaultSerializer);
            return data != null && data.Count > 0;
        }
    }
}
