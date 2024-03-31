using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    internal interface IFileCache: IDisposable
    {
        bool ContainsKey(string key);
        void Remove(string key);
        void Set(string key, byte[] fileData);
        Task SetAsync(string key, byte[] fileData);
        Task<byte[]> GetAsync(string key);
        byte[] Get(string key);
        void Flush();
    }
}
