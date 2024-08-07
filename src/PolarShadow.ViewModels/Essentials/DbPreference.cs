using Microsoft.EntityFrameworkCore;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PolarShadow.Core;
using PolarShadow.Services;

namespace PolarShadow.Essentials
{
    internal class DbPreference : IPreference
    {
        private readonly IPreferenceService _preferenceService;
        public DbPreference(IPreferenceService preference)
        {
            _preferenceService = preference;
        }
        public void Clear()
        {
            _preferenceService.Clear();
        }

        public async Task ClearAsync()
        {
            await _preferenceService.ClearAsync();
        }

        public T Get<T>(string key, T defaultValue)
        {
            var value = _preferenceService.Get(key);
            if (value == null) return defaultValue;

            return JsonSerializer.Deserialize<T>(value.Value, JsonOption.DefaultSerializer);
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue)
        {
            var value = await _preferenceService.GetAsync(key);
            if (value == null) return defaultValue;
            return JsonSerializer.Deserialize<T>(value.Value, JsonOption.DefaultSerializer);
        }

        public void Set<T>(string key, T value)
        {
            _preferenceService.Set(new PreferenceModel { Key = key, Value = JsonSerializer.Serialize(value, JsonOption.DefaultSerializer)});
        }

        public async Task SetAsync<T>(string key, T value)
        {
            await _preferenceService.SetAsync(new PreferenceModel { Key = key, Value = JsonSerializer.Serialize(value, JsonOption.DefaultSerializer) });
        }
    }
}
