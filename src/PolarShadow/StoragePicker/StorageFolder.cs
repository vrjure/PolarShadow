using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaIStorageFolder = Avalonia.Platform.Storage.IStorageFolder;
using AvaIStorageFile = Avalonia.Platform.Storage.IStorageFile;

namespace PolarShadow.StoragePicker
{
    internal class StorageFolder : IStorageFolder
    {
        internal AvaIStorageFolder _storageFolder;
        public StorageFolder(AvaIStorageFolder storageFolder)
        {
            _storageFolder = storageFolder;
        }

        public string Name => _storageFolder.Name;

        public Uri Uri => _storageFolder.Path;

        public async Task<IStorageFile> CreateFileAsync(string name)
        {
            var result = await _storageFolder.CreateFileAsync(name);
            return new StorageFile(result);
        }

        public async Task<IStorageFolder> CreateFolderAsync(string name)
        {
            var result = await _storageFolder.CreateFolderAsync(name);
            return new StorageFolder(result);
        }

        public async Task DeleteAsync()
        {
            await _storageFolder.DeleteAsync();
        }

        public async IAsyncEnumerable<IStorageItem> GetItemsAsync()
        {
            var items = _storageFolder.GetItemsAsync();
            await foreach (var item in items)
            {
                if (item is AvaIStorageFolder folder)
                {
                    yield return new StorageFolder(folder);
                }
                else if (item is AvaIStorageFile file)
                {
                    yield return new StorageFile(file);
                }
            }
        }

        public async Task<IStorageItem> MoveAsync(IStorageFolder destination)
        {
            var result = await _storageFolder.MoveAsync((destination as StorageFolder)._storageFolder);
            return new StorageFolder(result as AvaIStorageFolder);
        }
    }
}
