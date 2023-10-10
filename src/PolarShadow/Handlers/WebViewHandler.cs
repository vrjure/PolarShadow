using Avalonia.Controls;
using AvaloniaWebView;
using HtmlAgilityPack;
using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Handlers
{
    internal class WebViewHandler : RequestHandlerBase, IWebViewRequestHandler
    {
        private Panel _container;
        private IList<WebViewTab> _tabs = new List<WebViewTab>();
        public void SetContainer(Panel container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            _container = container;
        }

        protected override async Task<IObjectParameter> OnRequestAsync(IRequest request, IParameter parameter, CancellationToken cancellation)
        {
            var tab = GetIdleWebView();
            try
            {
                var url = request.Request.Url.Format(parameter).Format(parameter);
                var html = await tab.LoadAsync(new Uri(url), cancellation);
                html = Regex.Unescape(Regex.Unescape(html).Trim('"'));
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                return new ObjectParameter(new ParameterValue(new HtmlElement(doc.CreateNavigator())));
            }
            finally
            {
                lock (_tabs)
                {
                    if (tab.State == WebViewState.Idle)
                    {
                        tab.Close();
                        _tabs.Remove(tab);
                    }
                }
            }
        }

        private WebViewTab GetIdleWebView()
        {
            lock (_tabs)
            {
                WebViewTab idleTab = _tabs.FirstOrDefault(f => f.State == WebViewState.Idle);
                if (idleTab == null)
                {
                    idleTab = new WebViewTab(_container);
                    _tabs.Add(idleTab);
                }
                return idleTab;
            }
        }
    }
}
