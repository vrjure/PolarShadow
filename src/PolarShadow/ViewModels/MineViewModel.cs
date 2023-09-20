using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Options;

namespace PolarShadow.ViewModels
{
    public class MineViewModel : ViewModelBase
    {
        private readonly IStorageService _storage;
        private readonly INotificationManager _notify;
        private readonly IPreference _preference;
        public MineViewModel(IStorageService storage, INotificationManager notify, IPreference preference)
        {
            _storage = storage;
            _notify = notify;
            _preference = preference;
        }

        private string _rpc;
        public string RPC
        {
            get => _rpc;
            set => SetProperty(ref _rpc, value);
        }

        private string _downloadPath;
        public string DownloadPath
        {
            get => _downloadPath;
            set => SetProperty(ref _downloadPath, value);
        }

        private IAsyncRelayCommand _pickDownloadPathCommand;
        public IAsyncRelayCommand PickDownloadPathCommand => _pickDownloadPathCommand ??= new AsyncRelayCommand(PickDownloadFolder);

        private IAsyncRelayCommand _saveCommand;
        public IAsyncRelayCommand SaveCommand => _saveCommand ??= new AsyncRelayCommand(SaveAsync);

        private async Task PickDownloadFolder()
        {
            var folders = await _storage.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
            {
                AllowMultiple = false
            });

            if (folders == null || folders.Count == 0) return;

            DownloadPath = folders.First().Path.AbsolutePath;
        }

        protected override async void OnLoad()
        {
            RPC = await _preference.GetAsync(PreferenceOption.RPC, "");
            DownloadPath = await _preference.GetAsync(PreferenceOption.DownloadPath, "");
        }

        private async Task SaveAsync()
        {
            try
            {
                await _preference.SetAsync(PreferenceOption.RPC, RPC);
                await _preference.SetAsync(PreferenceOption.DownloadPath, DownloadPath);

                _notify.ShowSuccess();
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }
    }
}
