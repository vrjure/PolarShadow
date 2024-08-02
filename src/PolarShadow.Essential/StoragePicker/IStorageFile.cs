using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IStorageFile : IStorageItem
    {
        Task<Stream> ReadAsync();
        Task<Stream> WriteAsync();
    }
}
