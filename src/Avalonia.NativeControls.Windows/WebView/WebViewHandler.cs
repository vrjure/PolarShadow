using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls.Windows
{
    internal class WebViewHandler : ViewHandler<Avalonia.NativeControls.WebView, WebView>, IWebViewHandler
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
            e.RoutedEvent = NativeControls.WebView.NavigatedEvent;
            e.Source = VirtualView;
            VirtualView.RaiseEvent(e);
        }

        private void PlatformView_Navigating(object sender, WebViewNavigatingArgs e)
        {
            e.RoutedEvent = NativeControls.WebView.NavigatingEvent;
            e.Source = VirtualView;
            VirtualView.RaiseEvent(e);
        }

        private void PlatformView_LoadResource(object sender, WebViewLoadResourceArgs e)
        {
            e.RoutedEvent = NativeControls.WebView.LoadResourceEvent;
            e.Source = VirtualView;
            VirtualView.RaiseEvent(e);
        }
    }
}
