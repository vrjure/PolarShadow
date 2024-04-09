using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal sealed class App
    {
        public static string AppDataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PolarShadow");
        public static string ConfigFile => Path.Combine(AppDataFolder, "config.json");
        public static string DbFile => Path.Combine(AppDataFolder, "polar.db");
        public static string CacheFolder => Path.Combine(AppDataFolder, "cache");
        public static string PreferenceFolder => Path.Combine(AppDataFolder, "preference");
    }
}
