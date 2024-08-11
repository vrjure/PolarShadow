using PolarShadow.Services;
using PolarShadow.Services.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services.Http
{
    internal class PreferenceService : IHttpPreferenceService
    {
        private static string Preference = "Preference";
        private readonly HttpClient _client;
        public PreferenceService(HttpClient client)
        {
            _client = client;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public async Task ClearAsync()
        {
            var url = $"{Preference}/clear";
            var result = await _client.DeleteFromJsonAsync<Result>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        public PreferenceModel Get(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<PreferenceModel>?> GetAllAsync()
        {
            var url = $"{Preference}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<PreferenceModel>>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task<PreferenceModel?> GetAsync(string key)
        {
            var url = $"{Preference}/{key}";
            var result = await _client.GetFromJsonAsync<Result<PreferenceModel>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public void Set(PreferenceModel item)
        {
            throw new NotImplementedException();
        }

        public async Task SetAsync(PreferenceModel item)
        {
            var url = $"{Preference}";
            var response = await _client.PostAsJsonAsync(url, item, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>();
            result.ThrowIfUnsuccessful();
        }
    }
}
