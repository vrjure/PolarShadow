using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class PlayAbleDefault : IPlayAble
    {
        public bool CanPlay { get; set; }

        public PlayMode PlayMode { get; set; }

        public Task<VideoSource> GetPalySourceAsync(VideoEpisode episode)
        {
            throw new NotImplementedException();
        }
    }
}
