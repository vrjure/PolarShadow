using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Videos
{
    public class VideoSeason
    {
        public string Name { get; set; }
        public ICollection<VideoEpisode> Episodes { get; set; }
    }
}
