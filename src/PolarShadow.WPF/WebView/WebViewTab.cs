using CommunityToolkit.Mvvm.DependencyInjection;
using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PolarShadow.WebView
{
    internal class WebViewTab
    {
        private static CoreWebView2Environment _environment;
        private static Task<CoreWebView2Environment> _createWebViewTask;

        private Panel _container;
        private readonly WebView2 _webView;
        private TaskCompletionSource<string> _tcs;
        private TaskCompletionSource _sniffTask;
        private static TimeSpan _timeout = TimeSpan.FromSeconds(60);
        private Sniff _sniff;
        private ICollection<string> _sniffUrls = new List<string>();

        private bool _webViewInitialized = false;
        static WebViewTab()
        {
            CreateEnvironment();
        }

        public WebViewTab(Panel container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            _container = container;
            _webView = new WebView2 { Visibility = System.Windows.Visibility.Visible };
            _container.Children.Add(_webView);
            State = WebViewState.Idle;
            Reset();
        }

        private static async void CreateEnvironment()
        {
            string userDataFolder = null;
            var options = Ioc.Default.GetService<WebViewOptions>();
            if (!string.IsNullOrEmpty(options?.UserDataFolder))
            {
                userDataFolder = options.UserDataFolder;
            }
            _createWebViewTask = CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
            _environment = await _createWebViewTask;
        }

        private async Task InitializeAsync()
        {
            if (_environment == null)
            {
                await _createWebViewTask;
            }

            await _webView.EnsureCoreWebView2Async(_environment, _environment.CreateCoreWebView2ControllerOptions());
            _webView.CoreWebView2.NavigationStarting -= CoreWebView2_NavigationStarting;
            _webView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            _webView.CoreWebView2.NavigationCompleted -= CoreWebView2_NavigationCompleted;
            _webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            _webView.CoreWebView2.WebResourceRequested -= CoreWebView2_WebResourceRequested;
            _webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            _webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All, CoreWebView2WebResourceRequestSourceKinds.All);
        }

        private void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            
        }

        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Request.Uri) || _sniff == Sniff.None)
            {
                return;
            }

            if (_sniff == Sniff.M3U8)
            {
                MatchM3U8(e.Request.Uri);
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                _tcs.SetResult(await _webView.CoreWebView2.ExecuteScriptAsync("document.documentElement.innerHTML"));
            }
            else
            {
                _tcs.SetException(new WebException($"{_webView.CoreWebView2.Source}"));
            }
        }

        public WebViewState State { get; private set; }

        public void SetReady()
        {
            State = WebViewState.Ready;
            Reset();
        }

        private void Reset()
        {
            _sniff = Sniff.None;
            _sniffUrls?.Clear();
        }

        public async Task<HtmlDocument> LoadAsync(Uri source, CancellationToken cancellation = default)
        {
            if (State == WebViewState.Loading)
            {
                throw new InvalidOperationException("WebView is Loading");
            }

            State = WebViewState.Loading;

            if (!_webViewInitialized)
            {
                await InitializeAsync();
                _webViewInitialized = true;
            }

            try
            {
                _tcs = new TaskCompletionSource<string>();
                _webView.CoreWebView2.Navigate(source.ToString());
                var html = await _tcs.Task.WaitAsync(_timeout, cancellation);
                var doc = new HtmlDocument();
                doc.LoadHtml(Regex.Unescape(Regex.Unescape(html).Trim('"')));
                return doc;
            }
            finally
            {
                State = WebViewState.Idle;
                Reset();
            }
        }

        public async Task<HtmlDocument> LoadAsync(Uri source, Sniff sniff, CancellationToken cancellation = default)
        {
            if (State == WebViewState.Loading) throw new InvalidOperationException("WebView is Loading");
            State = WebViewState.Loading;

            if (!_webViewInitialized)
            {
                await InitializeAsync();
                _webViewInitialized = true;
            }

            try
            {
                _tcs = new TaskCompletionSource<string>();
                _sniff = sniff;
                if (_sniff != Sniff.None)
                {
                    _sniffTask = new TaskCompletionSource();
                }

                _webView.CoreWebView2.Navigate(source.ToString());
                var result = await _tcs.Task.WaitAsync(_timeout, cancellation);
                if (_sniff != Sniff.None)
                {
                    try
                    {
                        await _sniffTask.Task.WaitAsync(_timeout, cancellation);
                    }
                    catch { }
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(Regex.Unescape(result.Trim('"')));

                if (_sniff == Sniff.None)
                {
                    return doc;
                }

                var body = doc.DocumentNode.SelectSingleNode("//body");
                var sniffNode = HtmlNode.CreateNode("<div class='sniff'></div>");
                body.AppendChild(sniffNode);

                foreach (var item in _sniffUrls)
                {
                    sniffNode.AppendChild(HtmlNode.CreateNode($"<span>{item}</span>"));
                }
                return doc;
            }
            finally
            {
                State = WebViewState.Idle;
                Reset();
            }
        }

        public async void Close()
        {
            if (State == WebViewState.Loading)
            {
                try
                {
                    _webView.Stop();
                    await _tcs?.Task;
                    await _sniffTask?.Task;
                }
                catch { }
            }
            _webView.CoreWebView2.NavigationStarting -= CoreWebView2_NavigationStarting;
            _webView.CoreWebView2.NavigationCompleted -= CoreWebView2_NavigationCompleted;
            _webView.CoreWebView2.WebResourceRequested -= CoreWebView2_WebResourceRequested;
            _container.Children.Remove(_webView);
        }

        private void MatchM3U8(string uri)
        {
            System.Diagnostics.Trace.WriteLine(uri);
            var u = new Uri(uri);
            if (u.AbsolutePath.EndsWith(".m3u8", StringComparison.OrdinalIgnoreCase))
            {
                _sniffUrls.Add(uri);
                _sniffTask.TrySetResult();
            }
        }
    }
}
