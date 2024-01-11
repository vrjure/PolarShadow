using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Controls;
using System.Linq;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace PolarShadow.Handlers
{
    internal class WebViewTab
    {
        private Panel _container;
        private readonly WebView _webView;
        private TaskCompletionSource<string> _tcs;
        private TaskCompletionSource _sniffTask;
        private static TimeSpan _timeout = TimeSpan.FromSeconds(15);
        private Sniff _sniff;
        private ICollection<string> _sniffUrls = new List<string>();

        public WebViewTab(Panel container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            _container = container;
            _webView = new WebView { IsVisible = true };
            _webView.Navigated += _webView_Navigated;
            _webView.LoadResource += _webView_LoadResource;
            _container.Children.Add(_webView);
            State = WebViewState.Idle;
            Reset();
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
            try
            {
                _tcs = new TaskCompletionSource<string>();
                _webView.Url = source.ToString();
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
            try
            {
                _tcs = new TaskCompletionSource<string>();
                _sniff = sniff;
                if (_sniff != Sniff.None)
                {
                    _sniffTask = new TaskCompletionSource();
                }

                _webView.Url = source.ToString();
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

                foreach ( var item in _sniffUrls) 
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
            _webView.Navigated -= _webView_Navigated;
            _container.Children.Remove(_webView);
        }

        private async void _webView_Navigated(object sender, WebViewNavigatedArgs e)
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

        private void _webView_LoadResource(object sender, WebViewLoadResourceArgs e)
        {
            if (string.IsNullOrEmpty(e.Uri) || _sniff == Sniff.None)
            {
                return;
            }

            if (_sniff == Sniff.M3U8)
            {
                MatchM3U8(e.Uri);
            }
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
