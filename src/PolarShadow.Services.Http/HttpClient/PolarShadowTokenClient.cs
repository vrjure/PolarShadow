using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services.Http
{
    internal class PolarShadowTokenClient : IPolarShadowTokenClient
    {
        private static string _token = "Token";

        private readonly HttpClient _client;
        private readonly TokenClientOptions _options;
        private readonly IMemoryCache _cache;
        public PolarShadowTokenClient(HttpClient client, TokenClientOptions options, IMemoryCache cache)
        {
            _client = client;
            _options = options;
            _cache = cache;
        }

        public async Task<TokenModel?> GetTokenAsync()
        {
            var request = _options.TokenRequestCreator?.Invoke();
            var url = $"{_token}";
            var response = await _client.PostAsJsonAsync(url, request, JsonOptions.DefaultSerializer);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Result<TokenModel>>(JsonOptions.DefaultSerializer);
            result.ThrowIfUnsuccessful();

            return result?.Data;
        }
    }
}
