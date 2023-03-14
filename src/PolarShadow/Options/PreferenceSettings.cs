using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Options
{
    internal sealed class PreferenceSettings
    {
        public static string Rpc
        {
            get => Preferences.Default.Get(nameof(Rpc), "");
            set => Preferences.Default.Set(nameof(Rpc), value);
        }

        public static string DownloadPath
        {
            get => Preferences.Default.Get(nameof(DownloadPath), "");
            set => Preferences.Default.Set(nameof(DownloadPath), value);
        }
    }
}
