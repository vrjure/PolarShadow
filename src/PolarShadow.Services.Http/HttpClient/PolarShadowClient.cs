using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace PolarShadow.Services.Http
{
    internal class PolarShadowClient : IPolarShadowClient
    {
        private static string History = "History";
        private static string Preference = "Preference";
        public static string _mineResource = "MineResource";
        private static string _source = "Source";
        private static string Server = "Server";

        private readonly HttpClient _client;

        public PolarShadowClient(HttpClient client)
        {
            _client = client;
        }

        async Task IHistoryService.AddOrUpdateAsync(HistoryModel model)
        {
            var url = $"{History}/addOrUpdate";
            var resopnse = await _client.PostAsJsonAsync(url, model);
            resopnse.EnsureSuccessStatusCode();
            var result = await resopnse.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        async Task IHistoryService.DeleteAsync(long id)
        {
            var url = $"{History}/{id}";
            var result = await _client.DeleteFromJsonAsync<Result>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        async Task<HistoryModel?> IHistoryService.GetByIdAsync(long id)
        {
            var url = $"{History}/byId/{id}";
            var result = await _client.GetFromJsonAsync<Result<HistoryModel>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task<HistoryModel?> IHistoryService.GetByResourceNameAsync(string resourceName)
        {
            var url = $"{History}/byName/{resourceName}";
            var result = await _client.GetFromJsonAsync<Result<HistoryModel>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task<ICollection<HistoryModel>?> IHistoryService.GetListPageAsync(int page, int pageSize, string filter)
        {
            var url = $"{History}?page={page}&pageSize={pageSize}&filter={filter}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<HistoryModel>>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task ISyncAble<HistoryModel>.UploadAsync(ICollection<HistoryModel> data)
        {
            var url = $"{History}/upload";
            var response = await _client.PostAsJsonAsync(url, data, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        async Task<ICollection<HistoryModel>> ISyncAble<HistoryModel>.DownloadAsync(DateTime lastTime)
        {
            var url = $"{History}/download?lastTime={FormatTime(lastTime)}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<HistoryModel>>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result!.Data;
        }

        async Task<DateTime> ISyncAble<HistoryModel>.GetLastTimeAsync()
        {
            var url = $"{History}/lastTime";
            var result = await _client.GetFromJsonAsync<Result<DateTime>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result!.Data;
        }




        void IPreferenceService.Clear()
        {
            throw new NotImplementedException();
        }

        async Task IPreferenceService.ClearAsync()
        {
            var url = $"{Preference}/clear";
            var result = await _client.DeleteFromJsonAsync<Result>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        PreferenceModel IPreferenceService.Get(string key)
        {
            throw new NotImplementedException();
        }

        void IPreferenceService.Set(PreferenceModel item)
        {
            throw new NotImplementedException();
        }

        async Task IPreferenceService.SetAsync(PreferenceModel item)
        {
            var url = $"{Preference}";
            var response = await _client.PostAsJsonAsync(url, item, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>();
            result.ThrowIfUnsuccessful();
        }

        async Task<ICollection<PreferenceModel>?> IPreferenceService.GetAllAsync()
        {
            var url = $"{Preference}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<PreferenceModel>>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task<PreferenceModel?> IPreferenceService.GetAsync(string key)
        {
            var url = $"{Preference}/{key}";
            var result = await _client.GetFromJsonAsync<Result<PreferenceModel>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }



        async Task IMineResourceService.DeleteRootResourceAsync(long rootId)
        {
            var url = $"{_mineResource}/delete/{rootId}";
            var result = await _client.DeleteFromJsonAsync<Result>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
        }

        async Task<ResourceModel?> IMineResourceService.GetResourceAsync(long id)
        {
            var url = $"{_mineResource}/{id}";
            var result = await _client.GetFromJsonAsync<Result<ResourceModel>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task<ICollection<ResourceModel>?> IMineResourceService.GetRootChildrenAsync(long rootId)
        {
            var url = $"{_mineResource}/children/{rootId}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<ResourceModel>>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task<ICollection<ResourceModel>?> IMineResourceService.GetRootChildrenAsync(long rootId, int level)
        {
            var url = $"{_mineResource}/children/{rootId}/{level}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<ResourceModel>>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task<int> IMineResourceService.GetRootChildrenCountAsync(long rootId, int level)
        {
            var url = $"{_mineResource}/children/count/{rootId}/{level}";
            var result = await _client.GetFromJsonAsync<Result<int>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result!.Data;
        }

        async Task<ResourceModel?> IMineResourceService.GetRootResourceAsync(string resourceName, string site)
        {
            var url = $"{_mineResource}/{resourceName}/{site}";
            var result = await _client.GetFromJsonAsync<Result<ResourceModel>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task<ICollection<ResourceModel>?> IMineResourceService.GetRootResourcesAsync()
        {
            var url = $"{_mineResource}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<ResourceModel>>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task IMineResourceService.SaveResourceAsync(ResourceTreeNode tree)
        {
            var url = $"{_mineResource}/save";
            var message = await _client.PostAsJsonAsync(url, tree);
            message.EnsureSuccessStatusCode();
            var result = await message.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
        }

        async Task ISyncAble<ResourceModel>.UploadAsync(ICollection<ResourceModel> data)
        {
            var url = $"{_mineResource}/upload";
            var response = await _client.PostAsJsonAsync(url, data, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
        }

        async Task<ICollection<ResourceModel>> ISyncAble<ResourceModel>.DownloadAsync(DateTime lastTime)
        {
            var url = $"{_mineResource}/download?lastTime={FormatTime(lastTime)}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<ResourceModel>>>(url, JsonOptions.DefaultSerializer);
            result?.ThrowIfUnsuccessful();
            return result!.Data;
        }

        async Task<DateTime> ISyncAble<ResourceModel>.GetLastTimeAsync()
        {
            var url = $"{_mineResource}/lastTime";
            var result = await _client.GetFromJsonAsync<Result<DateTime>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result!.Data;
        }



        async Task<SourceModel?> ISourceService.GetSouuceAsync()
        {
            var url = _source;
            var result = await _client.GetFromJsonAsync<Result<SourceModel>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result?.Data;
        }

        async Task ISourceService.SaveSourceAsync(SourceModel source)
        {
            var url = _source;
            var response = await _client.PostAsJsonAsync(url, source, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        async Task ISyncAble<SourceModel>.UploadAsync(ICollection<SourceModel> data)
        {
            var url = $"{_source}/upload";
            var response = await _client.PostAsJsonAsync(url, data, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result>(JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
        }

        async Task<ICollection<SourceModel>> ISyncAble<SourceModel>.DownloadAsync(DateTime lastTime)
        {
            var url = $"{_source}/download?lastTime={FormatTime(lastTime)}";
            var result = await _client.GetFromJsonAsync<Result<ICollection<SourceModel>>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result!.Data;
        }

        async Task<DateTime> ISyncAble<SourceModel>.GetLastTimeAsync()
        {
            var url = $"{_source}/lastTime";
            var result = await _client.GetFromJsonAsync<Result<DateTime>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result!.Data;
        }


        public async ValueTask<DateTime> GetServerTimeAsync()
        {
            var url = $"{Server}/time";
            var result = await _client.GetFromJsonAsync<Result<DateTime>>(url, JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();
            return result!.Data;
        }

        private string FormatTime(DateTime time)
        {
            return time.ToUniversalTime().ToString();
        }
    }
}
