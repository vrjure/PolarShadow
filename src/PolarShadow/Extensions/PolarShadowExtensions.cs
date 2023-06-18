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
        public static void SaveToFile(this IPolarShadowOptionBuilder builder)
        {
            var sourcePath = Path.Combine(FileSystem.AppDataDirectory, _optionFileName);
            using var fs = new FileStream(sourcePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.SetLength(0);
            builder.WriteTo(fs);
        }

        public static void ReadFromFile(this IPolarShadowOptionBuilder builder)
        {
            var sourcePath = Path.Combine(FileSystem.AppDataDirectory, _optionFileName);
            using var fs = new FileStream(sourcePath, FileMode.OpenOrCreate, FileAccess.Read);
            if (fs.Length == 0)
            {
                return;
            }
            builder.Load(fs);
        }
    }
}
