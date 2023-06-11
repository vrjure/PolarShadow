using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Videos
{
    public class VideoDetail : VideoSummary
    {
        public VideoDetail()
        {

        }

        public ICollection<VideoSeason> Seasons { get; set; }
    }
}
