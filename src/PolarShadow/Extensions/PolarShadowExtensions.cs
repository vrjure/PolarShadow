using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal static class PolarShadowExtensions
    {
        public static void Save(this IPolarShadow polarShadow)
        {
            if (!Directory.Exists(App.AppDataFolder))
            {
                Directory.CreateDirectory(App.AppDataFolder);
            }
            
            if (!File.Exists(App.ConfigFile))
            {
                using (var fs = File.Create(App.ConfigFile))
                {
                    polarShadow.SaveTo(new JsonStreamSource(fs));
                }
            }
            else
            {
                polarShadow.SaveTo(new JsonFileSource() { Path = App.ConfigFile });
            }
        }

        public static void Load(this IPolarShadow polarShadow, bool reload = false)
        {
            if (!File.Exists(App.ConfigFile))
            {
                return;
            }
            polarShadow.Load(new JsonFileSource() { Path = App.ConfigFile }, reload);
        }
    }
}
