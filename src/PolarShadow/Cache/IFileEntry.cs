using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    internal interface IFileEntry : IDisposable
    {
        void WriteFile(byte[] fileData);
        Task WriteFileAsync(byte[] fileData);
        Task<byte[]> ReadFileAsync();
        byte[] ReadFile();
        void Dispose(bool delete);
    }
}
