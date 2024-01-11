using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public class WebViewNavigatingArgs : RoutedEventArgs
    {
        public WebViewNavigatingArgs(string url)
        {
            this.Url = url;
        }
        public string Url { get; }
    }
}
