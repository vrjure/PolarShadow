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
            if (!Directory.Exists(PolarShadowApp.AppDataFolder))
            {
                Directory.CreateDirectory(PolarShadowApp.AppDataFolder);
            }
            
            if (!File.Exists(PolarShadowApp.ConfigFile))
            {
                using (var fs = File.Create(PolarShadowApp.ConfigFile))
                {
                    polarShadow.SaveTo(new JsonStreamSource(fs));
                }
            }
            else
            {
                polarShadow.SaveTo(new JsonFileSource() { Path = PolarShadowApp.ConfigFile });
            }
        }

        public static void Load(this IPolarShadow polarShadow, bool reload = false)
        {
            if (!File.Exists(PolarShadowApp.ConfigFile))
            {
                return;
            }
            polarShadow.Load(new JsonFileSource() { Path = PolarShadowApp.ConfigFile }, reload);
        }
    }
}
