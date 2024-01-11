using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public class WebView : VirtualView, IWebViewVirtualView
    {
        public WebView() : base(NativeControls.GetHandler<IWebViewHandler>())
        {
            
        }

        public static readonly RoutedEvent<WebViewNavigatingArgs> NavigatingEvent = RoutedEvent.Register<WebView, WebViewNavigatingArgs>(nameof(Navigating), RoutingStrategies.Direct);
        public event EventHandler<WebViewNavigatingArgs> Navigating
        {
            add => AddHandler(NavigatingEvent, value);
            remove => RemoveHandler(NavigatingEvent, value);
        }

        public static readonly RoutedEvent<WebViewNavigatedArgs> NavigatedEvent = RoutedEvent.Register<WebView, WebViewNavigatedArgs>(nameof(Navigated), RoutingStrategies.Direct);
        public event EventHandler<WebViewNavigatedArgs> Navigated
        {
            add => AddHandler(NavigatedEvent, value);
            remove => RemoveHandler(NavigatedEvent, value);
        }

        public static readonly RoutedEvent<WebViewLoadResourceArgs> LoadResourceEvent = RoutedEvent.Register<WebView, WebViewLoadResourceArgs>(nameof(LoadResource), RoutingStrategies.Direct);
        public event EventHandler<WebViewLoadResourceArgs> LoadResource
        {
            add => AddHandler(LoadResourceEvent, value);
            remove => RemoveHandler(LoadResourceEvent, value);
        }

        public static readonly DirectProperty<WebView, string> UrlProperty = AvaloniaProperty.RegisterDirect<WebView, string>(nameof(Url), wv => wv.Url, (wv, v) => wv.Url = v, string.Empty);

        public string Url
        {
            get => this.Handler.PlatformView.Url;
            set
            {
                if (this.Handler == null)
                {
                    return;
                }

                this.Handler.PlatformView.Url = value;
            }
        }

        public new IWebViewHandler Handler => base.Handler as IWebViewHandler;

        public async Task<string> ExecuteScriptAsync(string script)
        {
            return await this.Handler?.PlatformView.ExecuteScriptAsync(script);
        }

        public void Stop()
        {
            this.Handler?.PlatformView.Stop();
        }
    }
}
