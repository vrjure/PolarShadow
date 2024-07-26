using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.StoragePicker
{
    internal class StorageFile : IStorageFile
    {
        public StorageFile(string filePath):this(new Uri(filePath))
        {

        }
        public StorageFile(Uri uri)
        {
            if (!uri.IsFile)
            {
                throw new ArgumentException("uri not a file");
            }
            this.Uri = uri;
            this.Name = Path.GetFileName(uri.ToString());
        }
        public string Name { get; }

        public Uri Uri { get; }

        public Task DeleteAsync()
        {
            File.Delete(Uri.ToString());
            return Task.CompletedTask;
        }

        public Task<IStorageItem> MoveAsync(IStorageFolder destination)
        {
            var dest = Path.Combine(destination.Uri.ToString(), Name);
            File.Move(Uri.ToString(), dest);
            return Task.FromResult<IStorageItem>(new StorageFile(dest));
        }

        public Task<Stream> ReadAsync()
        {
            return Task.FromResult<Stream>(new FileStream(Uri.ToString(), FileMode.OpenOrCreate, FileAccess.Read));
        }

        public Task<Stream> WriteAsync()
        {
            return Task.FromResult<Stream>(new FileStream(Uri.ToString(), FileMode.OpenOrCreate, FileAccess.Write));
        }
    }
}
