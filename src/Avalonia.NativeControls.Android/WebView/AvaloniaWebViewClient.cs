using Android.Graphics;
using Android.Webkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls.Android
{
    internal class AvaloniaWebViewClient : WebViewClient
    {
        public event EventHandler<WebViewNavigatingArgs> PageStarted;
        public event EventHandler<WebViewNavigatedArgs> PageFinished;
        public event EventHandler<string> LoadResource;
        public override void OnPageStarted(global::Android.Webkit.WebView view, string url, Bitmap favicon)
        {
            PageStarted?.Invoke(view, new WebViewNavigatingArgs(url));
        }

        public override void OnPageFinished(global::Android.Webkit.WebView view, string url)
        {
            PageFinished?.Invoke(view, new WebViewNavigatedArgs(200));
        }

        public override void OnLoadResource(global::Android.Webkit.WebView view, string url)
        {
            LoadResource?.Invoke(view,url);
        }

    }
}
