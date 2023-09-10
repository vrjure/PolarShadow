﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    internal interface IFileCache: IDisposable
    {
        string CacheFolder { get; }
        bool ContainsKey(string key);
        void Remove(string key);
        void Set(string key, byte[] fileData);
        Task SetAsync(string key, byte[] fileData);
        Task<byte[]> GetAsync(string key);
        byte[] Get(string key);
    }
}
