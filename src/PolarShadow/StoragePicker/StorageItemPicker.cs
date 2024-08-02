using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IStorageItem = PolarShadow.Essentials.IStorageItem;
using IStorageFile = PolarShadow.Essentials.IStorageFile;
using IStorageFolder = PolarShadow.Essentials.IStorageFolder;
using PickerOptions = PolarShadow.Essentials.PickerOptions;
using AvaIStorageFolder = Avalonia.Platform.Storage.IStorageFolder;
using System.Collections.ObjectModel;

namespace PolarShadow.StoragePicker
{
    internal class StorageItemPicker : IStorageItemPicker
    {
        private readonly ITopLevelService _topLevelService;

        public StorageItemPicker(ITopLevelService topLevelService)
        {
            _topLevelService = topLevelService;
        }

        public async Task<IReadOnlyList<IStorageItem>> OpenPickerAsync(PickerOptions options)
        {
            var result = new List<IStorageItem>();
            AvaIStorageFolder defaultFolder = null;
            if (!string.IsNullOrEmpty(options.DefaultLocation))
            {
                defaultFolder = await _topLevelService.GetTopLevel().StorageProvider.TryGetFolderFromPathAsync(options.DefaultLocation);
            }

            if (options.PickerType == PickerType.File)
            {
                var filePickerOptions = new FilePickerOpenOptions
                {
                    Title = options.Title,
                    AllowMultiple = options.AllowMultiple,
                    SuggestedStartLocation = defaultFolder
                };

                if (options.FileTypeFilter != null && options.FileTypeFilter.Count > 0)
                {
                    var filters = new List<FilePickerFileType>();
                    foreach (var item in options.FileTypeFilter)
                    {
                        var filter = new FilePickerFileType(item.Name)
                        {
                            Patterns = item.Patterns,
                            MimeTypes = item.MimeTypes
                        };
                        filters.Add(filter);
                    }
                    filePickerOptions.FileTypeFilter = filters;
                }

                var files = await _topLevelService.GetTopLevel().StorageProvider.OpenFilePickerAsync(filePickerOptions);
                if (files != null && files.Count > 0)
                {
                    foreach (var item in files)
                    {
                        result.Add(new StorageFile(item));
                    }
                }

            }
            else
            {
                var folderPickerOptions = new FolderPickerOpenOptions
                {
                    Title = options.Title,
                    AllowMultiple = options.AllowMultiple,
                    SuggestedStartLocation = defaultFolder
                };

                var folders = await _topLevelService.GetTopLevel().StorageProvider.OpenFolderPickerAsync(folderPickerOptions);
                if (folders != null && folders.Count > 0)
                {
                    foreach (var item in folders)
                    {
                        result.Add(new StorageFolder(item));
                    }
                }
            }

            return new ReadOnlyCollection<IStorageItem>(result);
        }

        public async Task<IStorageFile> SavePickerAsync(PickerOptions options)
        {
            AvaIStorageFolder defaultFolder = null;
            if (!string.IsNullOrEmpty(options.DefaultLocation))
            {
                defaultFolder = await _topLevelService.GetTopLevel().StorageProvider.TryGetFolderFromPathAsync(options.DefaultLocation);
            }

            var filePickerSaveOptioins = new FilePickerSaveOptions()
            {
                Title = options.Title,
                SuggestedStartLocation = defaultFolder,
            };

            if (options.FileTypeFilter != null && options.FileTypeFilter.Count > 0)
            {
                var filters = new List<FilePickerFileType>();
                foreach (var item in options.FileTypeFilter)
                {
                    var filter = new FilePickerFileType(item.Name)
                    {
                        Patterns = item.Patterns,
                        MimeTypes = item.MimeTypes
                    };
                    filters.Add(filter);
                }
                filePickerSaveOptioins.FileTypeChoices = filters;
            }

            var result = await _topLevelService.GetTopLevel().StorageProvider.SaveFilePickerAsync(filePickerSaveOptioins);
            if (result != null)
            {
                return new StorageFile(result);
            }

            return null;
        }
    }
}
