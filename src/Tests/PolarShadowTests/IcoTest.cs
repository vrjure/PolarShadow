using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Controls;

namespace PolarShadowTests
{
    public class IcoTest
    {
        private static string path = Path.Combine(Directory.GetCurrentDirectory(), "test.ico");
        [Test]
        public void TestReadInfo()
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var ico = new Ico(fs);
                Console.WriteLine(ico.ToString());
            }
        }

        [Test]
        public void TestChangeFormat()
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var ico = new Ico(fs);
                var index = ico.Count - 1;
                var ext = ico.GetFormat(index) switch
                {
                    ImageFormat.BMP => ".bmp",
                    ImageFormat.JPEG => ".jpg",
                    ImageFormat.PNG => ".png",
                    _ => ".unknown"
                };
                var generateFile = Path.Combine(Directory.GetCurrentDirectory(), $"testGenerate{ext}");
                if (File.Exists(generateFile))
                {
                    File.Delete(generateFile);
                }
                using(var wfs = new FileStream(generateFile, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    ico.GetStream(index).CopyTo(wfs);
                    wfs.Flush();
                }
            }
        }
    }
}
