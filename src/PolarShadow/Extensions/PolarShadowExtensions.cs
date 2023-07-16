using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal static class PolarShadowExtensions
    {
        private static string _optionFileName = "source.json";
        public static void SaveToFile(this IPolarShadow polaShadow)
        {
            var sourcePath = Path.Combine(FileSystem.AppDataDirectory, _optionFileName);
            polaShadow.SaveTo(new JsonFileSource() { Path = sourcePath });
        }

        public static void ReadFromFile(this IPolarShadow polarShadow)
        {
            var sourcePath = Path.Combine(FileSystem.AppDataDirectory, _optionFileName);
            polarShadow.Load(new JsonFileSource { Path = sourcePath}, true);
        }
    }
}
