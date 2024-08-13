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
    internal class MineResourceService : IHttpMineResourceService
    {
        public static string _mineResource = "MineResource";
        private readonly HttpClient _client;
        public MineResourceService(HttpClient client)
        {
            _client = client;
        }

        public async Task DeleteRootResourceAsync(int rootId)
        {
            var url = $"{_mineResource}/delete/{rootId}";
            var result = await _client.DeleteFromJsonAsync<Result>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
        }

        public async Task<ResourceModel?> GetResourceAsync(int id)
        {
            var url = $"{_mineResource}/{id}";
            var result = await _client.GetFromJsonAsync<Result<ResourceModel>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task<ICollection<ResourceModel>?> GetRootChildrenAsync(int rootId)
        {
            var url = $"{_mineResource}/children/{rootId}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<ResourceModel>>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task<ICollection<ResourceModel>?> GetRootChildrenAsync(int rootId, int level)
        {
            var url = $"{_mineResource}/children/{rootId}/{level}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<ResourceModel>>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task<int> GetRootChildrenCountAsync(int rootId, int level)
        {
            var url = $"{_mineResource}/children/count/{rootId}/{level}";
            var result = await _client.GetFromJsonAsync<Result<int>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result!.Data;
        }

        public async Task<ResourceModel?> GetRootResourceAsync(string resourceName, string site)
        {
            var url = $"{_mineResource}/{resourceName}/{site}";
            var result = await _client.GetFromJsonAsync<Result<ResourceModel>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task<ICollection<ResourceModel>?> GetRootResourcesAsync()
        {
            var url = $"{_mineResource}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<ResourceModel>>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        public async Task SaveResourceAsync(ResourceTreeNode tree)
        {
            var url = $"{_mineResource}/save";
            var message = await _client.PostAsJsonAsync(url, tree);
            message.EnsureSuccessStatusCode();
            var result = await message.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
        }

        public async Task UploadAsync(ICollection<ResourceModel> data)
        {
            var url = $"{_mineResource}/upload";
            var response = await _client.PostAsJsonAsync(url, data, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
        }

        public async Task<ICollection<ResourceModel>> DownloadAsync()
        {
            var url = $"{_mineResource}/download";
            var result = await _client.GetFromJsonAsync<Result<ICollection<ResourceModel>>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result!.Data;
        }
    }
}
