using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls.Windows
{
    internal class WebViewHandler : ViewHandler<Avalonia.Controls.WebView, WebView>, IWebViewHandler
    {
        IWebViewPlatformView IWebViewHandler.PlatformView => base.PlatformView as IWebViewPlatformView;

        protected override IPlatformView OnCreatePlatformView()
        {
            return new WebView(VirtualView);
        }

        protected override void ConnectHandler(WebView platformView)
        {
            platformView.Navigating += PlatformView_Navigating;
            platformView.Navigated += PlatformView_Navigated;
            platformView.LoadResource += PlatformView_LoadResource;
        }

        protected override void DisconnectHandler(WebView platformView)
        {
            platformView.Navigating -= PlatformView_Navigating;
            platformView.Navigated -= PlatformView_Navigated;
            platformView.LoadResource -= PlatformView_LoadResource;
        }

        private void PlatformView_Navigated(object sender, WebViewNavigatedArgs e)
        {
            e.RoutedEvent = Avalonia.Controls.WebView.NavigatedEvent;
            e.Source = VirtualView;
            VirtualView.RaiseEvent(e);
        }

        private void PlatformView_Navigating(object sender, WebViewNavigatingArgs e)
        {
            e.RoutedEvent = Avalonia.Controls.WebView.NavigatingEvent;
            e.Source = VirtualView;
            VirtualView.RaiseEvent(e);
        }

        private void PlatformView_LoadResource(object sender, WebViewLoadResourceArgs e)
        {
            e.RoutedEvent = Avalonia.Controls.WebView.LoadResourceEvent;
            e.Source = VirtualView;
            VirtualView.RaiseEvent(e);
        }
    }
}
