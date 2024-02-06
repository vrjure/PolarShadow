using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    internal class StorageService : IStorageService
    {
        private readonly ITopLevelService _topLevelService;

        public StorageService(ITopLevelService topLevelService)
        {
            _topLevelService = topLevelService;
        }

        public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
        {
            return _topLevelService.GetTopLevel().StorageProvider.OpenFilePickerAsync(options);
        }

        public Task<IStorageFile> SaveFilePickerAsync(FilePickerSaveOptions optioins)
        {
            return _topLevelService.GetTopLevel().StorageProvider.SaveFilePickerAsync(optioins);
        }

        public Task<IStorageFile> TryGetFileFromPathAsync(Uri filePath)
        {
            return _topLevelService.GetTopLevel().StorageProvider.TryGetFileFromPathAsync(filePath);
        }

        public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
        {
            return _topLevelService.GetTopLevel().StorageProvider.OpenFolderPickerAsync(options);
        }

        public Task<IStorageFolder> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder)
        {
            return _topLevelService.GetTopLevel().StorageProvider.TryGetWellKnownFolderAsync(wellKnownFolder);
        }
    }
}
