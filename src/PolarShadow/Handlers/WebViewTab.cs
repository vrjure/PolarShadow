using AvaloniaWebView;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Controls;
using WebViewCore.Events;

namespace PolarShadow.Handlers
{
    internal class WebViewTab
    {
        private Panel _container;
        private readonly WebView _webView;
        private TaskCompletionSource<string> _tcs;
        public WebViewTab(Panel container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            _container = container;
            _webView = new WebView { IsVisible = false };
            _webView.NavigationStarting += _webView_Navigating;
            _webView.NavigationCompleted += _webView_Navigated;
            State = WebViewState.Idle;
        }

        public WebViewState State { get; private set; }

        public Task<string> LoadAsync(Uri source, CancellationToken cancellation = default)
        {
            if (State == WebViewState.Loading)
            {
                throw new InvalidOperationException("WebView is Loading");
            }
            _container.Children.Add(_webView);
            _tcs = new TaskCompletionSource<string>();
            _webView.Url = source;
            return _tcs.Task.WaitAsync(cancellation);
        }

        private void _webView_Navigating(object sender, WebViewUrlLoadingEventArg e)
        {
            State = WebViewState.Loading;
        }

        private async void _webView_Navigated(object sender, WebViewUrlLoadedEventArg e)
        {
            try
            {
                if (e.IsSuccess)
                {
                    _tcs.SetResult(await _webView.ExecuteScriptAsync("document.documentElement.innerHTML"));
                }
                else
                {
                    _tcs.SetException(new WebException($"{_webView.Url}"));
                }
            }
            finally
            {
                _container.Children.Remove(_webView);
                State = WebViewState.Idle;
            }
        }
    }
}
