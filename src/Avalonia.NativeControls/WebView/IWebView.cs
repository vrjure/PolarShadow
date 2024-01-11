using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public interface IWebView
    {
        string Url { get; set; }
        void Stop();
        Task<string> ExecuteScriptAsync(string script);

        event EventHandler<WebViewNavigatingArgs> Navigating;
        event EventHandler<WebViewNavigatedArgs> Navigated;
        event EventHandler<WebViewLoadResourceArgs> LoadResource;
    }
}
