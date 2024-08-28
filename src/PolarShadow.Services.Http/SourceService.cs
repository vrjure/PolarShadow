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
        private readonly IPolarShadowClient _client;
        public SourceService(IPolarShadowClient client)
        {
            _client = client;
        }

        public async Task<SourceModel?> GetSouuceAsync()
        {
            return await _client.GetSouuceAsync();
        }

        public async Task SaveSourceAsync(SourceModel source)
        {
            await _client.SaveSourceAsync(source);
        }

        public async Task UploadAsync(ICollection<SourceModel> data)
        {
            await _client.UploadAsync(data);
        }

        public async Task<ICollection<SourceModel>> DownloadAsync(DateTime lastTime)
        {
            return await (_client as IHttpSourceService).DownloadAsync(lastTime);
        }

        public async Task<DateTime> GetLastTimeAsync()
        {
            return await (_client as IHttpSourceService).GetLastTimeAsync();
        }
    }
}
