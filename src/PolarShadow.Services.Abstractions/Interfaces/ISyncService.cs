using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface ISyncAble<T> where T : class
    {
        Task UploadAsync(ICollection<T> data);
        Task<ICollection<T>> DownloadAsync();
    }

    public interface ISyncService<T> where T : class
    {
        Task UploadAsync();
        Task<ICollection<T>> DownloadAsync();
        Task SyncAsync();
    }
}
