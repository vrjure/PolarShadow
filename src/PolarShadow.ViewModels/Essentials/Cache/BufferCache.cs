using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    internal class BufferCache : IBufferCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IFileCache _fileCache;
        private readonly TimeSpan _slidingExpiration = TimeSpan.FromMinutes(1);

        public BufferCache(IMemoryCache memoryCache, IFileCache fileCache)
        {
            _memoryCache = memoryCache;
            _fileCache = fileCache;
        }

        public bool ContainsKey(string key, BufferLocation location = BufferLocation.Both)
        {
            return location switch
            {
                BufferLocation.Memory => _memoryCache.TryGetValue(key, out _),
                BufferLocation.File => _fileCache.ContainsKey(key),
                _ => _memoryCache.TryGetValue(key, out _) || _fileCache.ContainsKey(key)
            };
        }

        public void Remove(string key, BufferLocation location = BufferLocation.Both)
        {
            switch (location)
            {
                case BufferLocation.Memory:
                    _memoryCache.Remove(key);
                    break;
                case BufferLocation.File:
                    _fileCache.Remove(key);
                    break;
                case BufferLocation.Both:
                    _memoryCache.Remove(key);
                    _fileCache.Remove(key);
                    break;
            }
        }

        public byte[] Get(string key, BufferLocation location = BufferLocation.Both)
        {

            return location switch
            {
                BufferLocation.Memory => _memoryCache.TryGetValue(key, out byte[] b1) ? b1 : null,
                BufferLocation.File => _fileCache.ContainsKey(key) ? _fileCache.Get(key) : null,
                _ => GetBothInternal(key)
            };

            byte[] GetBothInternal(string key)
            {
                if (_memoryCache.TryGetValue(key, out byte[] buffer)) return buffer;
                if (_fileCache.ContainsKey(key)) return _fileCache.Get(key);
                return null;
            }
        }

        public async Task<byte[]> GetAsync(string key, BufferLocation location = BufferLocation.Both)
        {
            return location switch
            {
                BufferLocation.Memory => _memoryCache.TryGetValue(key, out byte[] b1) ? b1 : null,
                BufferLocation.File => _fileCache.ContainsKey(key) ? await _fileCache.GetAsync(key) : null,
                _ => await GetBothInternalAsync(key)
            };

            async Task<byte[]> GetBothInternalAsync(string key)
            {
                if (_memoryCache.TryGetValue(key, out byte[] buffer)) return buffer;
                if (_fileCache.ContainsKey(key)) return await _fileCache.GetAsync(key);
                return null;
            }
        }

        public void Set(string key, byte[] buffer, BufferLocation location)
        {
            switch (location)
            {
                case BufferLocation.Memory:
                    SetMemoryBuffer(key, buffer);
                    break;
                case BufferLocation.File:
                    _fileCache.Set(key, buffer);
                    break;
                case BufferLocation.Both:
                    SetMemoryBuffer(key, buffer);
                    _fileCache.Set(key, buffer);
                    break;
            }
        }

        public async Task SetAsync(string key, byte[] buffer, BufferLocation location)
        {
            switch (location)
            {
                case BufferLocation.Memory:
                    SetMemoryBuffer(key, buffer);
                    break;
                case BufferLocation.File:
                    await _fileCache.SetAsync(key, buffer);
                    break;
                case BufferLocation.Both:
                    SetMemoryBuffer(key, buffer);
                    await _fileCache.SetAsync(key, buffer);
                    break;
                default:
                    break;
            }
        }

        private void SetMemoryBuffer(string key, byte[] buffer)
        {
            _memoryCache.Set(key, buffer, new MemoryCacheEntryOptions { SlidingExpiration = _slidingExpiration});
        }
    }
}
