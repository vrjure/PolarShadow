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
        public static async Task SaveToFileAsync(this IPolarShadowBuilder builder)
        {
            var sourcePath = Path.Combine(FileSystem.AppDataDirectory, _optionFileName);
            using var fs = new FileStream(sourcePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.SetLength(0);
            var json = JsonSerializer.Serialize(builder.Option, JsonOption.DefaultSerializer);
            var buffer = Encoding.UTF8.GetBytes(json);
            await fs.WriteAsync(buffer);
            await fs.FlushAsync();
        }

        public static IPolarShadowBuilder ReadFromFile(this IPolarShadowBuilder builder)
        {
            var sourcePath = Path.Combine(FileSystem.AppDataDirectory, _optionFileName);
            using var fs = new FileStream(sourcePath, FileMode.OpenOrCreate, FileAccess.Read);
            if (fs.Length == 0)
            {
                return builder;
            }
            else
            {
                builder.ImportFrom(fs);
            }

            return builder;
        }
    }
}
