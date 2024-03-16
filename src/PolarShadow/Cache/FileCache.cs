using Avalonia.Controls.Shapes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    internal class FileCache : IFileCache
    {
        private readonly ConcurrentDictionary<string, IFileEntry> _entryCache = new ConcurrentDictionary<string, IFileEntry>();
        private readonly FileCacheOptions _options;
        private long _totalSize;
        public FileCache(string cacheFolder) : this(new FileCacheOptions() { CacheFolder = cacheFolder})
        {
            
        }

        public FileCache(FileCacheOptions options)
        {
            _options = options;
            if (string.IsNullOrEmpty(_options.CacheFolder))
            {
                throw new InvalidOperationException("'CacheFolder' can not empty");
            }
            _totalSize = ReadCahceSize();
            
        }

        public bool ContainsKey(string key)
        {
            return _entryCache.ContainsKey(key);
        }

        public void Remove(string key)
        {
            if(_entryCache.TryRemove(key, out IFileEntry entry))
            {
                entry.Dispose(true);
            }
        }
        public void Set(string key, byte[] fileData)
        {
            if (!_entryCache.TryGetValue(key, out IFileEntry entry))
            {
                entry = new FileEntry(GetFilePath(key));
            }

            var old = entry.Size;
            entry.WriteFile(fileData);
            Interlocked.Add(ref _totalSize, fileData.Length - old);
        }


        public async Task SetAsync(string key, byte[] fileData)
        {
            if (!_entryCache.TryGetValue(key, out IFileEntry entry))
            {
                entry = new FileEntry(GetFilePath(key));                
            }

            var old = entry.Size;
            await entry.WriteFileAsync(fileData).ConfigureAwait(false);
            Interlocked.Add(ref _totalSize, fileData.Length - old);
            _entryCache.TryAdd(key, entry);
        }

        public byte[] Get(string key)
        {
            if (_entryCache.TryGetValue(key, out IFileEntry entry))
            {
                return entry.ReadFile();
            }

            throw new InvalidOperationException($"{key} not in cache");
        }

        public async Task<byte[]> GetAsync(string key)
        {
            if(_entryCache.TryGetValue(key, out IFileEntry entry))
            {
                return await entry.ReadFileAsync();
            }

            throw new InvalidOperationException($"{key} not in cache");
        }

        public void Flush()
        {
            ReleaseIf();
        }

        public void Dispose()
        {
            _entryCache.Clear();
        }

        private string GetFilePath(string key)
        {
            return System.IO.Path.Combine(_options.CacheFolder, $"{key}.cache");
        }

        private long ReadCahceSize()
        {
            var filesPaths = Directory.GetFiles(_options.CacheFolder);
            var total = 0L;
            foreach (var filePath in filesPaths)
            {
                var entry = _entryCache.GetOrAdd(filePath, (path) => new FileEntry(path));
                total += entry.Size;
            }

            return total;
        }

        private void ReleaseIf()
        {
            if (_options.MaxCacheSize <= 0) return;
            var total = Interlocked.Read(ref _totalSize);
            if (total > _options.MaxCacheSize)
            {
                var toDeleteSize = total - _options.MaxCacheSize;
                var deleteSize = 0L;
                var order = _entryCache.OrderByDescending(f => f.Value.CreateTime);
                foreach (var item in order)
                {
                    if(_entryCache.Remove(item.Key, out IFileEntry entry))
                    {
                        deleteSize += item.Value.Size;
                        entry.Dispose(true);
                        if (deleteSize > toDeleteSize)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
