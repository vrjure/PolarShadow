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

        public async Task ExecuteAsync(Stream output, AnalysisRequest request, AnalysisResponse response, IContentBuilder requestBuilder, IContentBuilder responseBuilder, IParameter parameter, CancellationToken cancellation = default)
        {
            if (request == null || response == null)
            {
                return;
            }
            await HandleValueAsync(output, request, response, requestBuilder, responseBuilder, parameter, cancellation);
        }

        private async Task HandleValueAsync(Stream output, AnalysisRequest request, AnalysisResponse response, IContentBuilder requestBuilder, IContentBuilder responseBuilder, IParameter parameter, CancellationToken cancellation)
        {
            if (request == null || response == null 
                || string.IsNullOrEmpty(request.Url))
            {
                return;
            }

            var url = request.Url.Format(parameter);
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
                requestBuilder?.BuildContent(ms, request.Body.Value, parameter);
                ms.Seek(0, SeekOrigin.Begin);
                requestMsg.Content = new StreamContent(ms);
            }

            using var responseMsg = await client.SendAsync(requestMsg, cancellation);
            responseMsg?.EnsureSuccessStatusCode();

            var contentType = responseMsg.Content.Headers.ContentType;
            var content = await responseMsg.Content.ReadAsStreamAsync();
            var inputContent = new Parameters(parameter);
            if (contentType.MediaType.Equals(_applicationjson, StringComparison.OrdinalIgnoreCase))
            {
                using var doc = JsonDocument.Parse(content);
                inputContent.Add(new ObjectParameter(new ParameterValue(doc.RootElement.Clone())));
            }
            else if (contentType.MediaType.Equals(_textHtml, StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(response.Encoding))
                {
                    var doc = new HtmlDocument();
                    doc.Load(content);
                    inputContent.Add(new ObjectParameter(new ParameterValue(new HtmlElement(doc.CreateNavigator()))));
                }
                else
                {
                    var doc = new HtmlDocument();
                    doc.Load(content, Encoding.GetEncoding(response.Encoding));
                    inputContent.Add(new ObjectParameter(new ParameterValue(new HtmlElement(doc.CreateNavigator()))));
                }
            }
            else
            {
                throw new InvalidOperationException($"Not supported content-type:{contentType}");
            }

            if (response.Template.HasValue)
            {
                responseBuilder?.BuildContent(output, response.Template.Value, inputContent);
            }
        }
    }
}
