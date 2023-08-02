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
    }
}
