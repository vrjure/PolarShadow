using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls
{
    public interface IWebView
    {
        string Url { get; set; }
        void Stop();
        event EventHandler<WebViewNavigatingArgs> Navigating;
        event EventHandler<WebViewNavigatedArgs> Navigated;
        Task<string> ExecuteScriptAsync(string script);
    }
}
