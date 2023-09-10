using Avalonia.Controls.Shapes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    internal class FileCache : IFileCache
    {
        private readonly ConcurrentDictionary<string, IFileEntry> _entryCache = new ConcurrentDictionary<string, IFileEntry>();

        public FileCache(string cacheFolder)
        {
            this.CacheFolder = cacheFolder;
        }
        public string CacheFolder { get; }

        public bool ContainsKey(string key)
        {
            if (_entryCache.ContainsKey(key)) return true;
            var path = GetFilePath(key);
            if (File.Exists(path))
            {
                _entryCache.GetOrAdd(key, f => new FileEntry(path));
                return true;
            }

            return false;
        }

        public void Remove(string key)
        {
            if(_entryCache.TryRemove(key, out IFileEntry entry))
            {
                entry.Dispose(true);
            }
        }

        public async Task SetAsync(string key, byte[] fileData)
        {
            var entry = _entryCache.GetOrAdd(key, k => new FileEntry(GetFilePath(key)));
            await entry.WriteFileAsync(fileData);
        }

        public async Task<byte[]> GetAsync(string key)
        {
            if(_entryCache.TryGetValue(key, out IFileEntry entry))
            {
                return await entry.ReadFileAsync();
            }

            throw new InvalidOperationException($"{key} not in cache");
        }

        public void Set(string key, byte[] fileData)
        {
            var entry = _entryCache.GetOrAdd(key, k => new FileEntry(GetFilePath(key)));
            entry.WriteFile(fileData);
        }

        public byte[] Get(string key)
        {
            if (_entryCache.TryGetValue(key, out IFileEntry entry))
            {
                return entry.ReadFile();
            }

            throw new InvalidOperationException($"{key} not in cache");
        }

        private string GetFilePath(string key)
        {
            return System.IO.Path.Combine(CacheFolder, $"{key}.cache");
        }

        public void Dispose()
        {
            var values =_entryCache.Values;
            _entryCache.Clear();
            foreach (var item in values)
            {
                item.Dispose();
            }

        }
    }
}
