using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    internal class StorageService : IStorageService
    {
        private readonly ITopLevelService _opLevelService;

        public StorageService(ITopLevelService topLevelService)
        {
            _opLevelService = topLevelService;
        }

        public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
        {
            return _opLevelService.GetTopLevel().StorageProvider.OpenFilePickerAsync(options);
        }

        public Task<IStorageFile> SaveFilePickerAsync(FilePickerSaveOptions optioins)
        {
            return _opLevelService.GetTopLevel().StorageProvider.SaveFilePickerAsync(optioins);
        }

        public Task<IStorageFile> TryGetFileFromPathAsync(Uri filePath)
        {
            return _opLevelService.GetTopLevel().StorageProvider.TryGetFileFromPathAsync(filePath);
        }
    }
}
