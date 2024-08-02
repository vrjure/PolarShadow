using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaIStorageFile = Avalonia.Platform.Storage.IStorageFile;

namespace PolarShadow.StoragePicker
{
    internal class StorageFile : IStorageFile
    {
        private AvaIStorageFile _storageFile;
        public StorageFile(AvaIStorageFile storagefile)
        {
            _storageFile = storagefile;
        }
        public string Name => _storageFile.Name;

        public Uri Uri => _storageFile.Path;

        public async Task DeleteAsync()
        {
            await _storageFile.DeleteAsync();
        }

        public async Task<IStorageItem> MoveAsync(IStorageFolder destination)
        {
            var result = await _storageFile.MoveAsync((destination as StorageFolder)._storageFolder);
            return new StorageFile(result as AvaIStorageFile);
        }

        public async Task<Stream> ReadAsync()
        {
            return await _storageFile.OpenReadAsync();
        }

        public async Task<Stream> WriteAsync()
        {
            return await _storageFile.OpenWriteAsync();
        }
    }
}
