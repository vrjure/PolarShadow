using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services.Http
{
    internal class SourceService : IHttpSourceService
    {
        private static string _source = "Source";
        private readonly HttpClient _client;
        public SourceService(HttpClient client)
        {
            _client = client;
        }

        public async Task<SourceModel?> GetSouuceAsync()
        {
            var url = _source;
            var result = await _client.GetFromJsonAsync<Result<SourceModel>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task SaveSourceAsync(SourceModel source)
        {
            var url = _source;
            var response = await _client.PostAsJsonAsync(url, source, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        public async Task UploadAsync(ICollection<SourceModel> data)
        {
            var url = $"{_source}/upload";
            var response = await _client.PostAsJsonAsync(url, data, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        public async Task<ICollection<SourceModel>> DownloadAsync()
        {
            var url = $"{_source}/download";
            var result = await _client.GetFromJsonAsync<Result<ICollection<SourceModel>>>(url);
            result.ThrowIfUnsuccessful();
            return result!.Data;
        }
    }
}
