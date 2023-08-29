using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Services;
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
        private readonly INotificationManager _notification;
        public BookSourceViewModel(IPolarShadow polar, IStorageService storage, INotificationManager notification)
        {
            _polar = polar;
            _storage = storage;
            _notification = notification;
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
            try
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

                _notification.ShowSuccess();
            }
            catch (Exception ex)
            {
                _notification.Show(ex);
            }    
        }

        private void Reflesh()
        {
            Sites = new ObservableCollection<ISite>(_polar.GetSites());
        }
    }
}
