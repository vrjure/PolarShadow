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
    internal class HistoryService : IHttpHistoryService
    {
        private static string History = "History";
        private readonly HttpClient _client;
        public HistoryService(HttpClient client)
        {
            _client = client;
        }
        public async Task AddOrUpdateAsync(HistoryModel model)
        {
            var url = $"{History}/addOrUpdate";
            var resopnse = await _client.PostAsJsonAsync(url, model);
            resopnse.EnsureSuccessStatusCode();
            var result = await resopnse.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
        }

        public async Task DeleteAsync(int id)
        {
            var url = $"{History}/{id}";
            var result = await _client.DeleteFromJsonAsync<Result>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        public async Task<HistoryModel?> GetByIdAsync(int id)
        {
            var url = $"{History}/byId/{id}";
            var result = await _client.GetFromJsonAsync<Result<HistoryModel>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task<HistoryModel?> GetByResourceNameAsync(string resourceName)
        {
            var url = $"{History}/byName/{resourceName}";
            var result = await _client.GetFromJsonAsync<Result<HistoryModel>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task<ICollection<HistoryModel>?> GetListPageAsync(int page, int pageSize, string filter = null)
        {
            var url = $"{History}?page={page}&pageSize={pageSize}&filter={filter}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<HistoryModel>>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task UploadAsync(ICollection<HistoryModel> data)
        {
            var url = $"{History}/upload";
            var response = await _client.PostAsJsonAsync(url, data, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }
    }
}
