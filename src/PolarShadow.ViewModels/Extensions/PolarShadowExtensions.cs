using PolarShadow.Core;
using PolarShadow.Services;
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

        public static async Task LoadAsync(this IPolarShadow polar, ISourceService sourceService, bool  reload = false)
        {
            var source = await sourceService.GetSouuceAsync();
            if (source != null)
            {
                await polar.LoadAsync(new JsonStringSource { Json = source.Data }, reload);
            }
        }
        public static async Task SaveAsync(this IPolarShadow polar, ISourceService sourceService)
        {
            using var ms = new MemoryStream();
            polar.SaveTo(new JsonStreamSource(ms));
            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms);
            var savedSource = await sourceService.GetSouuceAsync();
            if (savedSource == null)
            {
                savedSource = new SourceModel
                {
                    Data = sr.ReadToEnd()
                };
            }
            else
            {
                savedSource.Data = sr.ReadToEnd();
                savedSource.UpdateTime = DateTime.Now;
            }

            await sourceService.SaveSourceAsync(savedSource);
        }
    }
}
