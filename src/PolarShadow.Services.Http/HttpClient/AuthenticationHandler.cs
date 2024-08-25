using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services.Http
{
    internal class AuthenticationHandler : DelegatingHandler
    {
        private readonly IServiceProvider _sp;
        private readonly IMemoryCache _cache;

        private readonly string TokenCacheKey = "PolarShadowToken";
        private readonly string TokenCacheExpireTime = "PolarShadowToken.ExpireTime";
        public AuthenticationHandler(IServiceProvider sp, IMemoryCache cache)
        {
            _sp = sp;
            _cache = cache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            TokenModel? token = null;
            if (!IsExprited)
            {
                token = _cache.Get<TokenModel>(TokenCacheKey);
            }
            else
            {
                var tokenClient = _sp.GetRequiredService<IPolarShadowTokenClient>();
                token = await tokenClient.GetTokenAsync();
                if (token != null)
                {
                    _cache.Set(TokenCacheKey, token);
                    _cache.Set(TokenCacheExpireTime, DateTime.Now.AddMinutes(token.Expires));
                }
            }

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token?.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }

        private bool IsExprited
        {
            get
            {
                bool refreshToken = false;
                if (_cache.TryGetValue(TokenCacheExpireTime, out DateTime dateTime))
                {
                    if (dateTime - DateTime.Now < TimeSpan.FromSeconds(30))
                    {
                        refreshToken = true;
                    }
                }
                else
                {
                    refreshToken = true;
                }

                return refreshToken;
            }
        }
    }
    
}
