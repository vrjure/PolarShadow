using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services.Http
{
    internal class ServerService : IServerService
    {
        private static string Server = "Server";
        private readonly HttpClient _client;
        public ServerService(HttpClient client)
        {
            _client = client;
        }

        public async ValueTask<DateTime> GetServerTime()
        {
            var url = $"{Server}/time";
            var result = await _client.GetFromJsonAsync<Result<DateTime>>(url);
            result.ThrowIfUnsuccessful();
            return result!.Data;
        }
    }
}
