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
        private static readonly string _formUrlEncoded = "application/x-www-form-urlencoded";
        private static readonly string _applicationjson = "application/json";
        private static readonly string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62";

        protected readonly AnalysisAbility AbilityConfig;
        private static HtmlWeb _web;
        public AnalysisAbilityHandler(AnalysisAbility ability)
        {
            AbilityConfig = ability;
        }

        protected virtual async Task<TResult> HandleValueAsync<TInput, TResult>(TInput input)
        {
            if (AbilityConfig.ResponseAnalysis == null || AbilityConfig.ResponseAnalysis.Count == 0)
            {
                return default;
            }

            var doc = JsonDocument.Parse(JsonSerializer.Serialize(input, JsonOption.DefaultSerializer));

            var url = AbilityConfig.Url.NameSlot(doc.RootElement);

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
                                return jsondoc.RootElement.Analysis<TResult>(AbilityConfig.ResponseAnalysis);
                            }
                        }
                    }
                    break;
                case AnalysisType.Html:
                    if (_web == null)
                    {
                        _web = new HtmlWeb();
                    }

                    Encoding encoding = null;
                    if (!string.IsNullOrEmpty(AbilityConfig.Encoding))
                    {
                        encoding = Encoding.GetEncoding(AbilityConfig.Encoding);
                    }
                    var htmlDoc = await _web.LoadFromWebAsync(url, encoding);
                    return htmlDoc.DocumentNode.Analysis<TResult>(AbilityConfig.ResponseAnalysis);
                default:
                    break;
            }

            return default;
        }
    }
}
