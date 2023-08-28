using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public interface IStorageService
    {
        Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options);
        Task<IStorageFile> SaveFilePickerAsync(FilePickerSaveOptions optioins);
    }
}
