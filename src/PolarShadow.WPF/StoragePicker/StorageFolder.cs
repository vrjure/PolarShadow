using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.StoragePicker
{
    internal class StorageFolder : IStorageFolder
    {
        private DirectoryInfo _info;
        public StorageFolder(string folderPath) : this(new Uri(folderPath))
        {

        }

        public StorageFolder(Uri uri)
        {
            this.Uri = uri;
            this.Name = Path.GetFileName(uri.ToString());
            _info = new DirectoryInfo(uri.ToString());
        }
        public string Name { get; }

        public Uri Uri { get; }

        public Task<IStorageFile> CreateFileAsync(string name)
        {
            return Task.FromResult<IStorageFile>(new StorageFile(Path.Combine(Uri.ToString(), name)));
        }

        public Task DeleteAsync()
        {
            _info.Delete();
            return Task.CompletedTask;
        }

        public Task<IStorageFolder> CreateFolderAsync(string name)
        {
            return Task.FromResult<IStorageFolder>(new StorageFolder(Path.Combine(Uri.ToString(), name)));
        }

        public async IAsyncEnumerable<IStorageItem> GetItemsAsync()
        {
            var dirs = _info.EnumerateDirectories();
            await Task.CompletedTask;
            foreach (var item in dirs)
            {
                yield return new StorageFolder(item.FullName);
            }

            var files = _info.EnumerateFiles();
            foreach (var item in files)
            {
                yield return new StorageFile(item.FullName);
            }
        }

        public Task<IStorageItem> MoveAsync(IStorageFolder destination)
        {
            var dest = Path.Combine(destination.Uri.ToString(), Name);
            _info.MoveTo(dest);
            return Task.FromResult<IStorageItem>(new StorageFolder(dest));
        }
    }
}
