using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Options;
using PolarShadow.Models;

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

        private ChangeValue<string> _rpc;
        public ChangeValue<string> RPC
        {
            get => _rpc;
            set => SetProperty(ref _rpc, value);
        }

        private ChangeValue<string> _downloadPath;
        public ChangeValue<string> DownloadPath
        {
            get => _downloadPath;
            set => SetProperty(ref _downloadPath, value);
        }

        private ChangeValue<int> _searchTaskCount;
        public ChangeValue<int> SearchTaskCount
        {
            get => _searchTaskCount;
            set => SetProperty(ref _searchTaskCount, value);
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

            DownloadPath.Value = folders.First().Path.AbsolutePath;
        }

        protected override async void OnLoad()
        {
            RPC = new ChangeValue<string>(await _preference.GetAsync(PreferenceOption.RPC, ""));
            DownloadPath = new ChangeValue<string>(await _preference.GetAsync(PreferenceOption.DownloadPath, ""));
            SearchTaskCount = new ChangeValue<int>(await _preference.GetAsync(PreferenceOption.SearchTaskCount, 3));
        }

        private async Task SaveAsync()
        {
            try
            {
                if(RPC.IsChange)
                    await _preference.SetAsync(PreferenceOption.RPC, RPC.Value);

                if(DownloadPath.IsChange)
                    await _preference.SetAsync(PreferenceOption.DownloadPath, DownloadPath.Value);

                if (SearchTaskCount.IsChange)
                    await _preference.SetAsync(PreferenceOption.SearchTaskCount, SearchTaskCount.Value);

                _notify.ShowSuccess();
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }
    }
}
