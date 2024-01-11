using Android.Webkit;
using Avalonia.Android;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidWebView = Android.Webkit.WebView;

namespace Avalonia.Controls.Android
{
    internal class WebView : PlatformView, IWebViewPlatformView
    {
        private AndroidWebView _androidWebView;
        private AvaloniaWebViewClient _webViewClient;
        private WebChromeClient _webChromeClient;


        public string Url
        {
            get => _androidWebView.Url;
            set
            {
                _androidWebView.LoadUrl(value);
            }
        }

        public void Stop()
        {
            if (_androidWebView == null)
            {
                return;
            }

            _androidWebView.StopLoading();
        }

        public event EventHandler<WebViewNavigatingArgs> Navigating;
        public event EventHandler<WebViewNavigatedArgs> Navigated;
        public event EventHandler<WebViewLoadResourceArgs> LoadResource;

        public async Task<string> ExecuteScriptAsync(string script)
        {
            if (_androidWebView == null)
            {
                return null;
            }
            var tcs = new TaskCompletionSource<string>();

            _androidWebView.EvaluateJavascript(script, new JavaScriptValueCallback(value => tcs.SetResult(value.ToString())));

            return await tcs.Task;
        }

        protected override IPlatformHandle OnCreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
        {
            var context = (parent as AndroidViewControlHandle)?.View.Context ?? global::Android.App.Application.Context;
            _androidWebView = new AndroidWebView(context);

            _webViewClient = new AvaloniaWebViewClient();
            _webChromeClient = new WebChromeClient();

            _androidWebView.SetWebViewClient(_webViewClient);
            _androidWebView.SetWebChromeClient(_webChromeClient);

            _androidWebView.Settings.JavaScriptEnabled = true;
            _androidWebView.Settings.LoadsImagesAutomatically = false;
            _androidWebView.Settings.DomStorageEnabled = true;
            _androidWebView.Settings.SetSupportMultipleWindows(false);

            _webViewClient.PageStarted += Client_PageStarted;
            _webViewClient.PageFinished += Client_PageFinished;
            _webViewClient.LoadResource += Client_LoadResource;
                
            return new AndroidViewControlHandle(_androidWebView);
        }

        protected override void DestroyControl()
        {
            if (_webViewClient != null)
            {
                _webViewClient.PageStarted -= Client_PageStarted;
                _webViewClient.PageFinished -= Client_PageFinished;
                _webViewClient.LoadResource -= Client_LoadResource;
                _webViewClient = null;
            }

            if (_androidWebView != null)
            {
                _androidWebView.Dispose();
                _androidWebView = null;
                _webChromeClient = null;
            }
        }

        private void Client_PageStarted(object sender, WebViewNavigatingArgs e)
        {
            this.Navigating?.Invoke(this, e);
        }

        private void Client_PageFinished(object sender, WebViewNavigatedArgs e)
        {
            this.Navigated?.Invoke(this, e);    
        }

        private void Client_LoadResource(object sender, string e)
        {
            this.LoadResource?.Invoke(this, new WebViewLoadResourceArgs(e));
        }
    }
}
