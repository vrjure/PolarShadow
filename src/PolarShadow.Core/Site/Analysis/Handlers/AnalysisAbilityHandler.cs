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
    public abstract class AnalysisAbilityHandler
    {
        private static readonly string _formUrlEncoded = "application/x-www-form-urlencoded";
        private static readonly string _applicationjson = "application/json";
        private static readonly string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62";

        protected readonly AnalysisAbility _abilityConfig;
        private static HtmlWeb _web;
        public AnalysisAbilityHandler(AnalysisAbility ability)
        {
            _abilityConfig = ability;
        }

        protected virtual async Task<TResult> HandleValueAsync<TInput, TResult>(TInput input)
        {
            if (_abilityConfig == null || _abilityConfig.ResponseAnalysis == null || _abilityConfig.ResponseAnalysis.Count == 0)
            {
                return default;
            }

            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(input, JsonOption.DefaultSerializer));
            return await HandleValueAsync<TResult>(doc.RootElement.Clone(), _abilityConfig);         
        }

        private async Task<TResult> HandleValueAsync<TResult>(JsonElement input, AnalysisAbility ability)
        {
            if (ability.ResponseAnalysis == null || ability.ResponseAnalysis.Count == 0)
            {
                return default;
            }

            if (ability.AnalysisType == AnalysisType.Json)
            {
                return await HandleJsonAsync<TResult>(input, ability);
            }
            else if (ability.AnalysisType == AnalysisType.Html)
            {
                return await HandleHtmlAsync<TResult>(input, ability);
            }

            return default;
        }

        private async Task<TResult> HandleJsonAsync<TResult>(JsonElement input, AnalysisAbility ability)
        {
            var url = ability.Url;
            if (!string.IsNullOrEmpty(url))
            {
                url = url.NameSlot(input);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("user-agent", _userAgent);
                    client.DefaultRequestHeaders.Add("content-type", _applicationjson);
                    var method = HttpMethod.Get;
                    if (!string.IsNullOrEmpty(ability.Method))
                    {
                        method = new HttpMethod(ability.Method);
                    }

                    using (var request = new HttpRequestMessage(method, url))
                    {
                        var response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await request.Content.ReadAsStringAsync();
                            using var jsondoc = JsonDocument.Parse(json);
                            
                            var newInput = jsondoc.RootElement.Append(input);
                            if (ability.Next == null)
                            {
                                return newInput.Analysis<TResult>(ability.ResponseAnalysis);
                            }
                            else
                            {
                                return await HandleJsonAsync<TResult>(newInput, ability.Next);
                            }
                        }
                    }
                }
            }
            else
            {
                return input.Analysis<TResult>(ability.ResponseAnalysis);
            }

            return default;
        }

        private async Task<TResult> HandleHtmlAsync<TResult>(JsonElement input, AnalysisAbility ability)
        {
            var url = ability.Url;
            if (!string.IsNullOrEmpty(url))
            {
                url = url.NameSlot(input);
                if (_web == null)
                {
                    _web = new HtmlWeb();
                }

                Encoding encoding = null;
                if (!string.IsNullOrEmpty(ability.Encoding))
                {
                    encoding = Encoding.GetEncoding(ability.Encoding);
                }
                var htmlDoc = await _web.LoadFromWebAsync(url, encoding);

                if (ability.Next == null)
                {
                    return htmlDoc.DocumentNode.Analysis<TResult>(input, ability.ResponseAnalysis);
                }
                else
                {
                    using var ms = new MemoryStream();
                    htmlDoc.DocumentNode.Analysis(input, ms, ability.ResponseAnalysis);
                    ms.Seek(0, SeekOrigin.Begin);
                    using var doc = JsonDocument.Parse(ms);
                    return await HandleValueAsync<TResult>(doc.RootElement.Clone(), ability.Next);
                }
            }
            else
            {
                return await HandleJsonAsync<TResult>(input, ability);
            }
        }
    }
}
