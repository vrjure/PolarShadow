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
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public static class AnalysisAbilityHandler
    {
        private static readonly string _formUrlEncoded = "application/x-www-form-urlencoded";
        private static readonly string _applicationjson = "application/json";
        private static readonly string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62";
        private static HtmlWeb _web = new HtmlWeb();

        public static async Task<string> ExecuteAsync(this AnalysisAbility ability, string input, CancellationToken cancellation = default)
        {
            if (ability == null || ability.ResponseAnalysis == null || ability.ResponseAnalysis.Count == 0)
            {
                return default;
            }
            using var doc = JsonDocument.Parse(input);
            using var ms = new MemoryStream();
            await HandleValueAsync(doc.RootElement, ms, ability, cancellation);
            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static async Task<TOutput> ExecuteAsync<TInput, TOutput>(this AnalysisAbility ability, TInput input, CancellationToken cancellation = default)
        {
            if (ability == null || ability.ResponseAnalysis == null || ability.ResponseAnalysis.Count == 0)
            {
                return default;
            }

            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(input, JsonOption.DefaultSerializer));
            using var ms = new MemoryStream();
            await HandleValueAsync(doc.RootElement, ms, ability, cancellation);
            ms.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.Deserialize<TOutput>(ms, JsonOption.DefaultSerializer);
        }

        private static async Task HandleValueAsync(JsonElement input, Stream stream, AnalysisAbility ability, CancellationToken cancellation)
        {
            if (ability.ResponseAnalysis == null || ability.ResponseAnalysis.Count == 0)
            {
                return;
            }

            if (ability.AnalysisType == AnalysisType.Json)
            {
                await HandleJsonAsync(input, stream, ability, cancellation);
            }
            else if (ability.AnalysisType == AnalysisType.Html)
            {
                await HandleHtmlAsync(input, stream, ability, cancellation);
            }
        }

        private static async Task HandleJsonAsync(JsonElement input, Stream stream, AnalysisAbility ability, CancellationToken cancellation)
        {
            var url = ability.Url;
            if (!string.IsNullOrEmpty(url))
            {
                url = url.NameSlot(input);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("user-agent", _userAgent);
                    var method = HttpMethod.Get;
                    if (!string.IsNullOrEmpty(ability.Method))
                    {
                        method = new HttpMethod(ability.Method);
                    }

                    using (var request = new HttpRequestMessage(method, url))
                    {                       
                        var response = await client.SendAsync(request, cancellation);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            using var jsondoc = JsonDocument.Parse(json);
                            
                            if (ability.Next == null)
                            {
                                jsondoc.RootElement.Analysis(stream, ability.ResponseAnalysis);
                            }
                            else
                            {
                                var newInput = jsondoc.RootElement.Append(input);
                                await HandleJsonAsync(newInput, stream, ability.Next, cancellation);
                            }
                        }
                    }
                }
            }
            else
            {
                input.Analysis(stream, ability.ResponseAnalysis);
            }
        }

        private static async Task HandleHtmlAsync(JsonElement input, Stream stream, AnalysisAbility ability, CancellationToken cancellation)
        {
            var url = ability.Url;
            if (!string.IsNullOrEmpty(url))
            {
                url = url.NameSlot(input);

                Encoding encoding = null;
                if (!string.IsNullOrEmpty(ability.Encoding))
                {
                    encoding = Encoding.GetEncoding(ability.Encoding);
                }

                var htmlDoc = await _web.LoadFromWebAsync(url, encoding, cancellation);
                      
                if (ability.Next == null)
                {
                    htmlDoc.DocumentNode.Analysis(input, stream, ability.ResponseAnalysis);
                }
                else
                {
                    using var ms = new MemoryStream();
                    htmlDoc.DocumentNode.Analysis(input, ms, ability.ResponseAnalysis);
                    ms.Seek(0, SeekOrigin.Begin);
                    using var doc = JsonDocument.Parse(ms);
                    await HandleValueAsync(doc.RootElement, stream, ability.Next, cancellation);
                }
            }
            else
            {
                await HandleJsonAsync(input, stream, ability , cancellation);
            }
        }
    }
}
