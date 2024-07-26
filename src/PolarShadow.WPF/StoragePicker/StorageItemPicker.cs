using Microsoft.Extensions.Options;
using Microsoft.Win32;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.StoragePicker
{
    internal class StorageItemPicker : IStorageItemPicker
    {
        public Task<IReadOnlyList<IStorageItem>> OpenPickerAsync(PickerOptions options)
        {
            var result = new List<IStorageItem>();

            if (options.PickerType == PickerType.File)
            {
                var picker = new OpenFileDialog();
                picker.Multiselect = options.AllowMultiple;
                picker.Title = options.Title;
                picker.DefaultDirectory = options.DefaultLocation;
                if (options.FileTypeFilter != null)
                {
                    picker.Filter = FileTypeFilterToWindowsFilter(options.FileTypeFilter);
                }
                if (picker.ShowDialog() == true)
                {
                    if (picker.Multiselect)
                    {
                        foreach (var item in picker.FileNames)
                        {
                            result.Add(new StorageFile(item));
                        }
                    }
                    else
                    {
                        result.Add(new StorageFile(picker.FileName));
                    }
                }
            }
            else
            {
                var picker = new OpenFolderDialog();
                picker.Multiselect = options.AllowMultiple;
                picker.Title = options.Title;
                picker.DefaultDirectory = options.DefaultLocation;
                if (picker.ShowDialog() == true)
                {
                    if (picker.Multiselect)
                    {
                        foreach (var item in picker.FolderNames)
                        {
                            result.Add(new StorageFolder(item));
                        }
                    }
                    else
                    {
                        result.Add(new StorageFolder(picker.FolderName));
                    }
                }
                
            }

            return Task.FromResult<IReadOnlyList<IStorageItem>>(new ReadOnlyCollection<IStorageItem>(result));
        }

        public Task<IStorageFile> SavePickerAsync(PickerOptions options)
        {
            var picker = new SaveFileDialog();
            picker.Title = options.Title;
            picker.DefaultDirectory = options.DefaultLocation;
            picker.Filter = FileTypeFilterToWindowsFilter(options.FileTypeFilter);
            if (picker.ShowDialog() == true)
            {
                return Task.FromResult<IStorageFile>(new StorageFile(picker.FileName));
            }

            return null;
        }

        private static string FileTypeFilterToWindowsFilter(IReadOnlyList<FilePickType> filter)
        {
            var sb = new StringBuilder();

            if (filter != null)
            {
                foreach (var item in filter)
                {
                    sb.Append(item.Name);
                    if (item.Patterns != null)
                    {
                        sb.Append('|');
                        for (int i = 0; i < item.Patterns.Count; i++)
                        {
                            sb.Append(item.Patterns[i]);
                            if (i < item.Patterns.Count - 1)
                            {
                                sb.Append(';');
                            }
                        }
                    }
                }
            }
            if (sb.Length == 0)
            {
                sb.Append("All files|*.*");
            }

            return sb.ToString();
        }
    }
}
