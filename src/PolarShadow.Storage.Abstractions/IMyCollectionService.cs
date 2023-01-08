using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Core;

namespace PolarShadow.Storage
{
    public interface IMyCollectionService
    {
        Task<ICollection<VideoSummary>> GetMyCollectionAsync(int page, int pageSize);
        Task AddToMyCollectionAsync(VideoSummary summary);
        Task RemoveFromMyCollectionAsync(string name);
        Task<bool> HasAsync(VideoSummary summary);
    }
}
