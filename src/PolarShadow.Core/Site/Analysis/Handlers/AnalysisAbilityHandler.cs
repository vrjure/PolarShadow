using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal abstract class AnalysisAbilityHandler
    {
        protected readonly AnalysisAbility AbilityConfig;
        private static HtmlWeb _web;
        public AnalysisAbilityHandler(AnalysisAbility ability)
        {
            AbilityConfig = ability;
        }

        protected virtual async Task<T> HandleValueAsync<T>(HttpRequestMessage request)
        {
            if (AbilityConfig.Analysis == null || AbilityConfig.Analysis.Count == 0)
            {
                return default;
            }
            var formatUrl = AbilityConfig.Url.FormatUrl();
            switch (AbilityConfig.AnalysisType)
            {
                case AnalysisType.Json:
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62");
                        var method = HttpMethod.Get;
                        if (!string.IsNullOrEmpty(AbilityConfig.Method))
                        {
                            method = new HttpMethod(AbilityConfig.Method);
                        }

                        using (var request = new HttpRequestMessage(method, url))
                        {
                            var response = await client.SendAsync(request);
                            if (response.IsSuccessStatusCode) 
                            { 
                                var json = await request.Content.ReadAsStringAsync();
                                var jsondoc = JsonDocument.Parse(json);
                                return jsondoc.RootElement.Analysis<T>(AbilityConfig.Analysis);
                            }
                        }
                    }
                    break;
                case AnalysisType.Html:
                    if (_web == null)
                    {
                        _web = new HtmlWeb();
                    }

                    var doc = await _web.LoadFromWebAsync(request.RequestUri.ToString());
                    return doc.DocumentNode.Analysis<T>(AbilityConfig.Analysis);
                default:
                    break;
            }

            return default;
        }
    }
}
