using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    internal interface IFileEntry : IDisposable
    {
        DateTime CreateTime { get; }
        long Size { get; }
        void WriteFile(byte[] fileData);
        Task WriteFileAsync(byte[] fileData);
        Task<byte[]> ReadFileAsync();
        byte[] ReadFile();
        void Dispose(bool delete);
    }
}
