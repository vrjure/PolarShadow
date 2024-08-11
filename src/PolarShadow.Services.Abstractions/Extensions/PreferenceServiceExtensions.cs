using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public static class PreferenceServiceExtensions
    {
        public static T Get<T>(this IPreferenceService service, string key, T defaultValue)
        {
            var value = service.Get(key);
            if (value == null) return defaultValue;

            return JsonSerializer.Deserialize<T>(value.Value, JsonOptions.DefaultSerializer);
        }

        public static async Task<T> GetAsync<T>(this IPreferenceService service, string key, T defaultValue)
        {
            var value = await service.GetAsync(key);
            if (value == null) return defaultValue;
            return JsonSerializer.Deserialize<T>(value.Value, JsonOptions.DefaultSerializer);
        }

        public static void Set<T>(this IPreferenceService service, string key, T value)
        {
            service.Set(new PreferenceModel { Key = key, Value = JsonSerializer.Serialize(value, JsonOptions.DefaultSerializer) });
        }

        public static async Task SetAsync<T>(this IPreferenceService service, string key, T value)
        {
            await service.SetAsync(new PreferenceModel { Key = key, Value = JsonSerializer.Serialize(value, JsonOptions.DefaultSerializer) });
        }
    }
}
