using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface IStorageService
    {
        Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options);
        Task<IStorageFile> SaveFilePickerAsync(FilePickerSaveOptions optioins);
        Task<IStorageFile> TryGetFileFromPathAsync(Uri filePath);
        Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options);
        Task<IStorageFolder> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder);
    }
}
