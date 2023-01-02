using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Pages.ViewModels
{
    public partial class WebBrowserPageViewModel : ObservableObject, IQueryAttributable
    {
        public readonly static string Key_url = "key_url";
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(Key_url, out object val))
            {
                WebViewSource = val.ToString();
            }
        }

        [ObservableProperty]
        private string webViewSource;
    }
}
