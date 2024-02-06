using Microsoft.EntityFrameworkCore;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PolarShadow.Core;

namespace PolarShadow.Essentials
{
    internal class DbPreference : IPreference
    {
        private readonly PreferenceManager _manager;
        public DbPreference(IDbContextFactory<PolarShadowDbContext> _dbContextFactory)
        {
            _manager = new PreferenceManager(_dbContextFactory);
        }
        public void Clear()
        {
            _manager.Clear();
        }

        public async Task ClearAsync()
        {
            await _manager.ClearAsync();
        }

        public T Get<T>(string key, T defaultValue)
        {
            var value = _manager.Get(key);
            if (value == null) return defaultValue;

            return JsonSerializer.Deserialize<T>(value.Value, JsonOption.DefaultSerializer);
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue)
        {
            var value = await _manager.GetAsync(key);
            if (value == null) return defaultValue;
            return JsonSerializer.Deserialize<T>(value.Value, JsonOption.DefaultSerializer);
        }

        public void Set<T>(string key, T value)
        {
            _manager.Set(new PreferenceEntity { Key = key, Value = JsonSerializer.Serialize(value, JsonOption.DefaultSerializer)});
        }

        public async Task SetAsync<T>(string key, T value)
        {
            await _manager.SetAsync(new PreferenceEntity { Key = key, Value = JsonSerializer.Serialize(value, JsonOption.DefaultSerializer) });
        }
    }
}
