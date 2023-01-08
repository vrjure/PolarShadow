using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal sealed class PolarShadowSetting
    {
        public static bool UseServer
        {
            get => Preferences.Get("useServer", false);
            set => Preferences.Set("useServer", value);
        }

        public static string Server
        {
            get => Preferences.Get("server", "localhost");
            set => Preferences.Set("server", value);
        }
    }
}
