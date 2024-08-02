using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IStorageFolder : IStorageItem
    {
        IAsyncEnumerable<IStorageItem> GetItemsAsync();
        Task<IStorageFile> CreateFileAsync(string name);
        Task<IStorageFolder> CreateFolderAsync(string name);
        
    }
}
