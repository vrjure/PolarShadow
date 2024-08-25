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
        private readonly IPolarShadowClient _client;
        public PreferenceService(IPolarShadowClient client)
        {
            _client = client;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public async Task ClearAsync()
        {
            await _client.ClearAsync();
        }

        public PreferenceModel Get(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<PreferenceModel>?> GetAllAsync()
        {
            return await _client.GetAllAsync();
        }

        public async Task<PreferenceModel?> GetAsync(string key)
        {
            return await _client.GetAsync(key);
        }

        public void Set(PreferenceModel item)
        {
            throw new NotImplementedException();
        }

        public async Task SetAsync(PreferenceModel item)
        {
            await _client.SetAsync(item);
        }
    }
}
