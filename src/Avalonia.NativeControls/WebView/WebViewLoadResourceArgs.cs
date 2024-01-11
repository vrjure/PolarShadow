using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public class WebViewLoadResourceArgs : RoutedEventArgs
    {
        public WebViewLoadResourceArgs(string uri)
        {
            this.Uri = uri;
        }

        public string Uri { get; }
    }
}
