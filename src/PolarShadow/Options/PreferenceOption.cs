using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Options
{
    public sealed class PreferenceOption
    {
        public static string RPC => nameof(RPC);
        public static string DownloadPath => nameof(DownloadPath);
    }
}
