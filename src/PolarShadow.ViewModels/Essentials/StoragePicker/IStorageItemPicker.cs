using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    public interface IStorageItemPicker
    {
        Task<IReadOnlyList<IStorageItem>> OpenPickerAsync(PickerOptions options);
        Task<IStorageFile> SavePickerAsync(PickerOptions optioins);
    }
}
