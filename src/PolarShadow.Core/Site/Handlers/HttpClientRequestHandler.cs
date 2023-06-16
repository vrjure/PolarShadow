using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class HttpClientRequestHandler : IRequestHandler
    {
        private static readonly string _applicationjson = "application/json";
        private static readonly string _textHtml = "text/html";
        private static readonly string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62";

        public async Task ExecuteAsync(AnalysisAbility ability, Stream stream, NameSlotValueCollection input, CancellationToken cancellation = default)
        {
            if (ability == null || ability.Request == null || ability.Response == null)
            {
                return;
            }
            await HandleValueAsync(input, stream, ability, cancellation);
            stream.Seek(0, SeekOrigin.Begin);
        }

        private async Task HandleValueAsync(NameSlotValueCollection input, Stream stream, AnalysisAbility ability, CancellationToken cancellation)
        {
            if (ability.Request == null || ability.Response == null 
                || string.IsNullOrEmpty(ability.Request.Url))
            {
                return;
            }

            var url = ability.Request.Url.Format(input);
            using var client = new HttpClient();

            if (!client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            }

            var method = ability.Request.Method;
            if (string.IsNullOrEmpty(method))
            {
                method = "get";
            }

            using var request = new HttpRequestMessage(new HttpMethod(method), url);

            if (ability.Request.Headers != null && ability.Request.Headers.Count > 0)
            {
                foreach (var item in ability.Request.Headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }
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
            var newInput = input.Clone();
            if (contentType.MediaType.Equals(_applicationjson, StringComparison.OrdinalIgnoreCase))
            {
                using var doc = JsonDocument.Parse(content);
                newInput.Add(doc.RootElement.Clone());
            }
            else if (contentType.MediaType.Equals(_textHtml, StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(ability.Response.Encoding))
                {
                    var doc = new HtmlDocument();
                    doc.Load(content);
                    newInput.Add(new HtmlElement(doc.CreateNavigator()));
                }
                else
                {
                    var doc = new HtmlDocument();
                    doc.Load(content, Encoding.GetEncoding(ability.Response.Encoding));
                    newInput.Add(new HtmlElement(doc.CreateNavigator()));
                }
            }
            else
            {
                throw new InvalidOperationException($"Not supported content-type:{contentType}");
            }

            ability.Response.Content.BuildContent(stream, newInput);
        }
    }
}
