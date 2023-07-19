using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal class WebViewTab
    {
        private IContainer _container;
        private readonly WebView _webView;
        private TaskCompletionSource<string> _tcs;
        public WebViewTab(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            _container = container;
            _webView = new WebView { IsVisible = false };
            _webView.Navigating += _webView_Navigating;
            _webView.Navigated += _webView_Navigated;
            State = WebViewState.Idle;
        }

        public WebViewState State { get; private set; }

        public Task<string> LoadAsync(WebViewSource source, CancellationToken cancellation = default)
        {
            if (State == WebViewState.Loading)
            {
                throw new InvalidOperationException("WebView is Loading");
            }
            _container.Add(_webView);
            _tcs = new TaskCompletionSource<string>();
            _webView.Source = source;
            return _tcs.Task.WaitAsync(cancellation);
        }

        private void _webView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            State = WebViewState.Loading;
        }

        private async void _webView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            try
            {
                if (e.Result == WebNavigationResult.Success)
                {
                    _tcs.SetResult(await _webView.EvaluateJavaScriptAsync("document.documentElement.innerHTML"));
                }
                else if (e.Result == WebNavigationResult.Timeout)
                {
                    _tcs.SetException(new TimeoutException(e.Url));
                }
                else if (e.Result == WebNavigationResult.Cancel)
                {
                    _tcs.SetCanceled();
                }
                else
                {
                    _tcs.SetException(new WebException($"{e.Url} {e.Result}"));
                }
            }
            finally
            {
                _container.Remove(_webView);
                State = WebViewState.Idle;
            }
        }
    }
}
