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
        private readonly IPolarShadowClient _client;
        public MineResourceService(IPolarShadowClient client)
        {
            _client = client;
        }

        public async Task DeleteRootResourceAsync(int rootId)
        {
            await _client.DeleteRootResourceAsync(rootId);
        }

        public async Task<ResourceModel?> GetResourceAsync(int id)
        {
            return await _client.GetResourceAsync(id);
        }

        public async Task<ICollection<ResourceModel>?> GetRootChildrenAsync(int rootId)
        {
            return await _client.GetRootChildrenAsync(rootId);
        }

        public async Task<ICollection<ResourceModel>?> GetRootChildrenAsync(int rootId, int level)
        {
            return await _client.GetRootChildrenAsync(rootId, level);
        }

        public async Task<int> GetRootChildrenCountAsync(int rootId, int level)
        {
            return await _client.GetRootChildrenCountAsync(rootId, level);
        }

        public async Task<ResourceModel?> GetRootResourceAsync(string resourceName, string site)
        {
            return await _client.GetRootResourceAsync(resourceName, site);
        }

        public async Task<ICollection<ResourceModel>?> GetRootResourcesAsync()
        {
            return await _client.GetRootResourcesAsync();
        }

        public async Task SaveResourceAsync(ResourceTreeNode tree)
        {
            await _client.SaveResourceAsync(tree);
        }

        public async Task UploadAsync(ICollection<ResourceModel> data)
        {
            await _client.UploadAsync(data);
        }

        public async Task<ICollection<ResourceModel>> DownloadAsync()
        {
            return await (_client as IHttpMineResourceService).DownloadAsync();
        }
    }
}
