using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls
{
    public class WebViewNavigatedArgs : RoutedEventArgs
    {
        public WebViewNavigatedArgs(int httpStatusCode)
        {
            this.HttpStatusCode = httpStatusCode;   
        }
        public int HttpStatusCode { get; }

        public bool IsSuccess => HttpStatusCode == 200;
    }
}
