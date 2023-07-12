using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core.Aria2
{
    public sealed class Aria2Http : IAria2Client, IDisposable
    {
        private readonly HttpClient _client;

        public Aria2Http(Uri address):this(address, default)
        {
            
        }

        public Aria2Http(Uri address, string secret) : this(address, secret, "PolarShadow")
        {
        }

        public Aria2Http(Uri address, string secret, string id) 
        {
            _client = new HttpClient();
            _client.BaseAddress = address;
            this.Secret = secret;
            this.Id = id;
        }

        public Uri Uri => _client.BaseAddress;
        public string Secret { get; }
        public string Id { get; }

        public Aria2Request CreateRequest()
        {
            return new Aria2Request(Id, Secret);
        }

        public async Task<Aria2Response<TResult>> PostAsync<TResult>(Aria2Request request, CancellationToken cancellationToken = default)
        {
            var content = new StringContent(JsonSerializer.Serialize(request, JsonOption.ForDashSerializer), Encoding.UTF8, "application/json");

            var result = await _client.PostAsync("", content, cancellationToken);
            result.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<Aria2Response<TResult>>(await result.Content.ReadAsStreamAsync(), JsonOption.ForDashSerializer);
        }

        public async Task<Aria2Response<TResult>> PostAsync<TResult>(Aria2Request<TResult> request, CancellationToken cancellationToken = default)
        {
            return await PostAsync<TResult>(request as Aria2Request, cancellationToken);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
