using PolarShadow.Core;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    internal class RemoteMyCollectionService : IRemoteMyCollectionService
    {
        private readonly HttpClient _client;
        public RemoteMyCollectionService(HttpClient client)
        {
            _client = client;
        }
        public async Task AddToMyCollectionAsync(VideoSummary summary)
        {
            await _client.PostAsJsonAsync("/add", summary, JsonOption.DefaultSerializer);
        }

        public async Task<ICollection<VideoSummary>> GetMyCollectionAsync(int page, int pageSize)
        {
            return await _client.GetFromJsonAsync<ICollection<VideoSummary>>($"/list?page={page}&pageSize={pageSize}", JsonOption.DefaultSerializer);
        }

        public Task<bool> HasAsync(VideoSummary summary)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveFromMyCollectionAsync(string name)
        {
            await _client.DeleteAsync("/delete");
        }
    }
}
