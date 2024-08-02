using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IPreference
    {
        void Set<T>(string key, T value);
        Task SetAsync<T>(string key, T value);
        T Get<T>(string key, T defaultValue);
        Task<T> GetAsync<T>(string key, T defaultValue);
        void Clear();
        Task ClearAsync();
    }
}
