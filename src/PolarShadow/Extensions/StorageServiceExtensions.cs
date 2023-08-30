using Avalonia.Platform.Storage;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Platform.Storage
{
    internal static class StorageServiceExtensions
    {
        public static async Task<IStorageFile> OpenPolarShadowConfigFilePickerAsync(this IStorageService storage)
        {
            var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Json File",
                AllowMultiple = false,
                FileTypeFilter = new[]
                    {
                        new FilePickerFileType("json")
                        {
                            Patterns = new[] { "*.json"},
                            MimeTypes = new[] {"application/json"}
                        }
                    }
            });

            if (files == null || files.Count == 0) return null;
            return files[0];
        }
    }
}
