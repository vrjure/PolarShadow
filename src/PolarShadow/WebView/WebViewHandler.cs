using HtmlAgilityPack;
using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PolarShadow
{
    public class WebViewHandler : IWebViewRequestHandler
    {
        private IContainer _container;
        private IList<WebViewTab> _tabs = new List<WebViewTab>();
        public void SetContainer(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container)); 
            _container = container;
        }

        public async Task ExecuteAsync(Stream output, AnalysisRequest request, AnalysisResponse response, IParameter input, CancellationToken cancellation = default)
        {
            if (request == null || response == null
                || string.IsNullOrEmpty(request.Url))
            {
                return;
            }

            var webView = GetIdleWebView();
            try
            {
                var url = request.Url.Format(input);
                var html = await webView.LoadAsync(url, cancellation);
                html = Regex.Unescape(Regex.Unescape(html).Trim('"'));
                var newInput = new Parameters(input);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                newInput.Add(new ObjectParameter(new ParameterValue(new HtmlElement(doc.CreateNavigator()))));

                response.Template?.BuildContent(output, newInput, input);
            }
            finally
            {
                lock (_tabs)
                {
                    if (webView.State == WebViewState.Idle)
                    {
                        _tabs.Remove(webView);
                    }
                }
            }
            
        }

        private WebViewTab GetIdleWebView()
        {
            lock (_tabs)
            {
                WebViewTab idleWebView = _tabs.FirstOrDefault(f=>f.State == WebViewState.Idle);
                if (idleWebView == null)
                {
                    idleWebView = new WebViewTab(_container);
                    _tabs.Add(idleWebView);
                }
                return idleWebView;
            }
        }
    }
}
