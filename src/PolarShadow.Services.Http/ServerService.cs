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
        private readonly IPolarShadowClient _client;
        public ServerService(IPolarShadowClient client)
        {
            _client = client;
        }

        public async ValueTask<DateTime> GetServerTimeAsync()
        {
            return await _client.GetServerTimeAsync();
        }
    }
}
