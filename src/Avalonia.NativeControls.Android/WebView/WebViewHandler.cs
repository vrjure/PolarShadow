using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls.Android
{
    internal class WebViewHandler : ViewHandler<NativeControls.WebView, WebView>, IWebViewHandler
    {
        IWebViewPlatformView IWebViewHandler.PlatformView => base.PlatformView as IWebViewPlatformView;

        protected override IPlatformView OnCreatePlatformView()
        {
            return new WebView();
        }

        protected override void ConnectHandler(WebView platformView)
        {
            platformView.Navigating += PlatformView_Navigating;
            platformView.Navigated += PlatformView_Navigated;
        }

        protected override void DisconnectHandler(WebView platformView)
        {
            platformView.Navigating -= PlatformView_Navigating;
            platformView.Navigated -= PlatformView_Navigated;
        }

        private void PlatformView_Navigating(object sender, WebViewNavigatingArgs e)
        {
            e.RoutedEvent = NativeControls.WebView.NavigatingEvent;
            e.Source = VirtualView;
            VirtualView.RaiseEvent(e);
        }

        private void PlatformView_Navigated(object sender, WebViewNavigatedArgs e)
        {
            e.RoutedEvent = NativeControls.WebView.NavigatedEvent;
            e.Source = VirtualView;
            VirtualView.RaiseEvent(e);
        }
    }
}
