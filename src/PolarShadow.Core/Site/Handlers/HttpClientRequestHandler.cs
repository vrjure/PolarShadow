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

        public async Task ExecuteAsync(Stream stream, AnalysisRequest request, AnalysisResponse response, NameSlotValueCollection input, CancellationToken cancellation = default)
        {
            if (request == null || response == null)
            {
                return;
            }
            await HandleValueAsync(input, stream, request, response, cancellation);
            stream.Seek(0, SeekOrigin.Begin);
        }

        private async Task HandleValueAsync(NameSlotValueCollection input, Stream stream, AnalysisRequest request, AnalysisResponse response, CancellationToken cancellation)
        {
            if (request == null || response == null 
                || string.IsNullOrEmpty(request.Url))
            {
                return;
            }

            var url = request.Url.Format(input);
            using var client = new HttpClient();

            if (!client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            }

            var method = request.Method;
            if (string.IsNullOrEmpty(method))
            {
                method = HttpMethod.Get.Method;
            }

            using var requestMsg = new HttpRequestMessage(new HttpMethod(method), url);

            if (request.Headers != null && request.Headers.Count > 0)
            {
                foreach (var item in request.Headers)
                {
                    requestMsg.Headers.Add(item.Key, item.Value);
                }
            }
            using var ms = new MemoryStream();
            if (request.Body.HasValue)
            {
                request.Body.Value.BuildContent(ms, input, input);
                requestMsg.Content = new StreamContent(ms);
            }

            using var responseMsg = await client.SendAsync(requestMsg, cancellation);
            responseMsg?.EnsureSuccessStatusCode();

            var contentType = responseMsg.Content.Headers.ContentType;
            var content = await responseMsg.Content.ReadAsStreamAsync();
            var newInput = input.Clone();
            if (contentType.MediaType.Equals(_applicationjson, StringComparison.OrdinalIgnoreCase))
            {
                using var doc = JsonDocument.Parse(content);
                newInput.Add(doc.RootElement.Clone());
            }
            else if (contentType.MediaType.Equals(_textHtml, StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(response.Encoding))
                {
                    var doc = new HtmlDocument();
                    doc.Load(content);
                    newInput.Add(new HtmlElement(doc.CreateNavigator()));
                }
                else
                {
                    var doc = new HtmlDocument();
                    doc.Load(content, Encoding.GetEncoding(response.Encoding));
                    newInput.Add(new HtmlElement(doc.CreateNavigator()));
                }
            }
            else
            {
                throw new InvalidOperationException($"Not supported content-type:{contentType}");
            }

            response.Template?.BuildContent(stream, newInput, input);
        }
    }
}
