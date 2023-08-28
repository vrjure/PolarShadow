using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class BookSourceViewModel : ViewModelBase
    {
        private readonly IPolarShadow _polar;
        private readonly IStorageService _storage;
        public BookSourceViewModel(IPolarShadow polar, IStorageService storage)
        {
            _polar = polar;
            _storage = storage;
        }
        private ObservableCollection<ISite> _sites;
        public ObservableCollection<ISite> Sites
        {
            get => _sites;
            set => SetProperty(ref _sites, value);
        }

        private IAsyncRelayCommand _importCommand;
        public IAsyncRelayCommand ImportCommand => _importCommand ??= new AsyncRelayCommand(ImportSource);


        public override void OnLoad()
        {
            Reflesh();
        }

        private async Task ImportSource()
        {
            var files = await _storage.OpenFilePickerAsync(new FilePickerOpenOptions
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

            if (files == null || files.Count == 0) return;

            _polar.LoadJsonStreamSource(await files[0].OpenReadAsync());

            Reflesh();
        }

        private void Reflesh()
        {
            Sites = new ObservableCollection<ISite>(_polar.GetSites());
        }
    }
}
