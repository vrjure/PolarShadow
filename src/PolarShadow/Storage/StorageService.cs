using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class StorageService : IStorageService
    {
        public TopLevel TopLevel { get; set; }
        public Func<Visual> VisualFactory { get; set; }

        public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
        {
            return GetTopLevel().StorageProvider.OpenFilePickerAsync(options);
        }

        public Task<IStorageFile> SaveFilePickerAsync(FilePickerSaveOptions optioins)
        {
            return GetTopLevel().StorageProvider.SaveFilePickerAsync(optioins);
        }

        private TopLevel GetTopLevel()
        {
            var topLevel = TopLevel;
            if (topLevel == null)
                topLevel = TopLevel.GetTopLevel(VisualFactory?.Invoke());

            return topLevel;
        }
    }
}
