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
        private static string AppDataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PolarShadow");
        private static string ConfigFile => Path.Combine(AppDataFolder, "config.json");
        public static void Save(this IPolarShadow polarShadow)
        {
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }
            
            if (!File.Exists(ConfigFile))
            {
                using (var fs = File.Create(ConfigFile))
                {
                    polarShadow.SaveTo(new JsonStreamSource(fs));
                }
            }
            else
            {
                polarShadow.SaveTo(new JsonFileSource() { Path = ConfigFile });
            }
        }

        public static void Load(this IPolarShadow polarShadow)
        {
            if (!File.Exists(ConfigFile))
            {
                return;
            }
            polarShadow.Load(new JsonFileSource() { Path = ConfigFile });
        }
    }
}
