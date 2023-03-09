using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal static class FileCacheHelper
    {
        private static readonly string _imageCacheDir = Path.Combine(FileSystem.CacheDirectory, "images");
        public static bool IsImageCached(this string fileName)
        {
            return File.Exists(Path.Combine(_imageCacheDir, fileName));
        }


        public static async Task CacheImageAsync(this string fileName, Stream stream)
        {
            var filePath = Path.Combine(_imageCacheDir, fileName);
            if (!Directory.Exists(_imageCacheDir))
            {
                Directory.CreateDirectory(_imageCacheDir);
            }
            FileStream fs = null;
            if (File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            }

            using (fs)
            {
                await stream.CopyToAsync(fs);
            }
        }

        public static Stream ReadStream(this string fileName)
        {
            var path = Path.Combine(_imageCacheDir, fileName);
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}
