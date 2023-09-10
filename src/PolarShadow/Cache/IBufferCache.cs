using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    public interface IBufferCache
    {
        bool ContainsKey(string key, BufferLocation location = BufferLocation.Both);
        Task<byte[]> GetAsync(string key, BufferLocation location = BufferLocation.Both);
        byte[] Get(string key, BufferLocation location = BufferLocation.Both);
        Task SetAsync(string key, byte[] buffer, BufferLocation location);
        void Set(string key, byte[] buffer, BufferLocation location);
        void Remove(string key, BufferLocation location = BufferLocation.Both);
    }

    public enum BufferLocation
    {
        Memory,
        File,
        Both
    }
}
