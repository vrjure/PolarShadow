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
using System.Xml.XPath;

namespace PolarShadow.Core
{
    public static class AnalysisAbilityHandler
    {
        private static readonly string _formUrlEncoded = "application/x-www-form-urlencoded";
        private static readonly string _applicationjson = "application/json";
        private static readonly string _textHtml = "text/html";
        private static readonly string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62";


        public static async Task ExecuteAsync(this AnalysisAbility ability, Stream stream, NameSlotValueCollection input, CancellationToken cancellation = default)
        {
            if (ability == null || ability.Request == null || ability.Response == null)
            {
                return;
            }

            if (ability.Parameters.HasValue)
            {
                input.AddNameValue(ability.Parameters.Value);
            }

            await HandleValueAsync(input, stream, ability, cancellation);
            stream.Seek(0, SeekOrigin.Begin);
        }
        public static async Task<string> ExecuteAsync(this AnalysisAbility ability, NameSlotValueCollection input, CancellationToken cancellation = default)
        {
            if (ability == null || ability.Request == null || ability.Response == null)
            {
                return default;
            }
            using var ms = new MemoryStream();
            await ExecuteAsync(ability, ms, input, cancellation);
            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static async Task<TOutput> ExecuteAsync<TOutput>(this AnalysisAbility ability, NameSlotValueCollection input, CancellationToken cancellation = default)
        {
            if (ability == null || ability.Request == null || ability.Response == null)
            {
                return default;
            }

            using var ms = new MemoryStream();
            await ExecuteAsync(ability, ms, input, cancellation);
            return JsonSerializer.Deserialize<TOutput>(ms, JsonOption.DefaultSerializer);
        }

        private static async Task HandleValueAsync(NameSlotValueCollection input, Stream stream, AnalysisAbility ability, CancellationToken cancellation)
        {
            if (ability.Request == null || ability.Response == null 
                || string.IsNullOrEmpty(ability.Request.Url) || string.IsNullOrEmpty(ability.Request.Method))
            {
                return;
            }

            var url = ability.Request.Url.Format(input);
            using var client = new HttpClient();
            if (ability.Request.Headers != null && ability.Request.Headers.Count > 0)
            {
                foreach (var item in ability.Request.Headers)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value.Format(input));
                }
            }

            var method = ability.Request.Method;

            using var request = new HttpRequestMessage(new HttpMethod(method), url);
            using var ms = new MemoryStream();
            if (ability.Request.Body.HasValue)
            {
                ability.Request.Body.Value.BuildContent(ms, input);
                request.Content = new StreamContent(ms);
            }

            using var response = await client.SendAsync(request, cancellation);
            response?.EnsureSuccessStatusCode();

            var contentType = response.Content.Headers.ContentType;
            var content = await response.Content.ReadAsStreamAsync();
            var newInput = new NameSlotValueCollection(new Dictionary<string, NameSlotValue>(input.Parameters));
            if (contentType.MediaType.Equals(_applicationjson, StringComparison.OrdinalIgnoreCase))
            {
                using var doc = JsonDocument.Parse(content);
                newInput.Add(doc.RootElement.Clone());
            }
            else if (contentType.MediaType.Equals(_textHtml, StringComparison.OrdinalIgnoreCase))
            {
                var doc = new XPathDocument(content);
                newInput.Add(new HtmlElement(doc));
            }
            else
            {
                throw new HttpRequestException($"Not supported content-type:{contentType}");
            }

            ability.Response.Content.BuildContent(stream, newInput);
        }
    }
}
