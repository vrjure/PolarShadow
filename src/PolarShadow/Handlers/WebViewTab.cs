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
        private static TimeSpan _timeout = TimeSpan.FromSeconds(15);
        public WebViewTab(Panel container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            _container = container;
            _webView = new WebView { IsVisible = false };
            _webView.NavigationCompleted += _webView_Navigated;
            _container.Children.Add(_webView);
            State = WebViewState.Idle;
        }

        public WebViewState State { get; private set; }

        public async Task<string> LoadAsync(Uri source, CancellationToken cancellation = default)
        {
            if (State == WebViewState.Loading)
            {
                throw new InvalidOperationException("WebView is Loading");
            }
            State = WebViewState.Loading;
            try
            {
                _tcs = new TaskCompletionSource<string>();
                _webView.Url = source;
                return await _tcs.Task.WaitAsync(_timeout, cancellation);
            }
            finally
            {
                State = WebViewState.Idle;
            }
        }

        public async void Close()
        {
            if (State == WebViewState.Loading)
            {
                try
                {
                    _webView.Stop();
                    await _tcs.Task;
                }
                catch { }
            }
            _webView.NavigationCompleted -= _webView_Navigated;
            _container.Children.Remove(_webView);
        }

        private async void _webView_Navigated(object sender, WebViewUrlLoadedEventArg e)
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
    }
}
