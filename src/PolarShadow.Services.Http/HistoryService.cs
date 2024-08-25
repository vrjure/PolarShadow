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
        private readonly IPolarShadowClient _client;
        public HistoryService(IPolarShadowClient client)
        {
            _client = client;
        }
        public async Task AddOrUpdateAsync(HistoryModel model)
        {
            await _client.AddOrUpdateAsync(model);
        }

        public async Task DeleteAsync(int id)
        {
            await _client.DeleteAsync(id);
        }

        public async Task<HistoryModel?> GetByIdAsync(int id)
        {
            return await _client.GetByIdAsync(id);
        }

        public async Task<HistoryModel?> GetByResourceNameAsync(string resourceName)
        {
            return await _client.GetByResourceNameAsync(resourceName);
        }

        public async Task<ICollection<HistoryModel>?> GetListPageAsync(int page, int pageSize, string? filter = null)
        {
            return await _client.GetListPageAsync(page, pageSize, filter);
        }

        public async Task UploadAsync(ICollection<HistoryModel> data)
        {
            await _client.UploadAsync(data);
        }

        public async Task<ICollection<HistoryModel>> DownloadAsync()
        {
            return await (_client as IHttpHistoryService).DownloadAsync();
        }
    }
}
