using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPlayAble
    {
        bool CanPlay { get; }
        PlayMode PlayMode { get; }
        Task<VideoSource> GetPalySourceAsync(VideoEpisode episode);
    }
}
