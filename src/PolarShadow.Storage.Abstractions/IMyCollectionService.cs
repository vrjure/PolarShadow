using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Core;
using PolarShadow.Videos;

namespace PolarShadow.Storage
{
    public interface IMyCollectionService
    {
        Task<ICollection<VideoDetail>> GetMyCollectionAsync(int page, int pageSize);
        Task AddToMyCollectionAsync(VideoDetail summary);
        Task RemoveFromMyCollectionAsync(string name);
        Task<bool> HasAsync(VideoSummary summary);
    }
}
