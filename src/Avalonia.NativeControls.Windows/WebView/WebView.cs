using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Drawing;
using Avalonia.Controls;
using System.Net;

namespace Avalonia.NativeControls.Windows
{
    internal class WebView : PlatformView, IWebViewPlatformView
    {
        private static CoreWebView2Environment _environment;
        private static Task<CoreWebView2Environment> _createWebViewTask;

        private CoreWebView2Controller _controller;
        private readonly IVirtualView _virtualView;

        static WebView()
        {
            CreateEnvironment();
        }

        public WebView(IVirtualView virtualView)
        {
            _virtualView = virtualView;
            _virtualView.AsHost().SizeChanged += WebView_SizeChanged;
        }

        private string _url;
        public string Url
        {
            get => _controller?.CoreWebView2?.Source;
            set
            {
                _url = value;
                if (_controller == null)
                {
                    return;
                }
                _controller.CoreWebView2.Navigate(value);
            }
        }

        public void Stop()
        {
            if (_controller == null)
            {
                return;
            }

            _controller.CoreWebView2.Stop();
        }

        public event EventHandler<WebViewNavigatingArgs> Navigating;
        public event EventHandler<WebViewNavigatedArgs> Navigated;
        public event EventHandler<WebViewLoadResourceArgs> LoadResource;

        protected override IPlatformHandle OnCreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
        {
            var handler = createDefault();

            if (_controller == null)
            {
                CreateController(handler.Handle);
            }
            else if (_controller.ParentWindow != handler.Handle)
            {
                _controller.ParentWindow = handler.Handle;
            }
            return handler;
        }

        protected override void DestroyControl()
        {
            if (_controller != null)
            {
                _controller.ParentWindow = IntPtr.Zero;
                _controller.CoreWebView2.NavigationStarting -= CoreWebView2_NavigationStarting;
                _controller.CoreWebView2.FrameNavigationCompleted -= CoreWebView2_NavigationCompleted;
                _controller.CoreWebView2.WebResourceRequested -= CoreWebView2_WebResourceRequested;
                //_controller.CoreWebView2.WebResourceResponseReceived -= CoreWebView2_WebResourceResponseReceived;
                _controller.Close();
                _controller = null;
            }
        }

        private static async void CreateEnvironment()
        {
            _createWebViewTask = CoreWebView2Environment.CreateAsync();
            _environment = await _createWebViewTask;
        }

        private async void CreateController(IntPtr handle)
        {
            if (_environment == null)
            {
                await _createWebViewTask;
            }

            Debug.Assert(_environment != null);

            _controller = await _environment.CreateCoreWebView2ControllerAsync(handle, _environment.CreateCoreWebView2ControllerOptions());
            _controller.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            _controller.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

            //requests in iframe is not be got
            _controller.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            //_controller.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
            _controller.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All, CoreWebView2WebResourceRequestSourceKinds.All);
            SetSize();
            if (!string.IsNullOrEmpty(_url))
            {
                _controller.CoreWebView2.Navigate(_url);
            }
        }


        private void CoreWebView2_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            LoadResource?.Invoke(this, new WebViewLoadResourceArgs(e.Request.Uri));
        }

        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            LoadResource?.Invoke(this, new WebViewLoadResourceArgs(e.Request.Uri));
        }

        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {           
            this.Navigated?.Invoke(this, new WebViewNavigatedArgs(e.HttpStatusCode));
        }

        private void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var arg = new WebViewNavigatingArgs(e.Uri);
            this.Navigating?.Invoke(this, arg);
            e.Cancel = arg.Handled;
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            return await _controller.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void WebView_SizeChanged(object sender, Controls.SizeChangedEventArgs e)
        {
            SetSize();
        }

        private void SetSize()
        {
            if (_controller == null)
            {
                return;
            }
            var scale = 1d;
            var toplevel = TopLevel.GetTopLevel(_virtualView.AsHost());
            if (toplevel != null)
            {
                scale = toplevel.RenderScaling;
            }

            _controller.Bounds = new Rectangle(0, 0, Convert.ToInt32(_virtualView.AsHost().Bounds.Width * scale), Convert.ToInt32(_virtualView.AsHost().Bounds.Height * scale));
        }
    }
}
