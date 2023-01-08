using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class VideoEpisode
    {
        public VideoSummary Summary { get; internal set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public ICollection<VideoSource> Sources { get; set; }
    }
}
