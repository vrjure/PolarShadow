using CommunityToolkit.Mvvm.Input;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Essentials;
using PolarShadow.Models;

namespace PolarShadow.ViewModels
{
    public class MineViewModel : ViewModelBase
    {
        private readonly IStorageItemPicker _storage;
        private readonly IMessageService _notify;
        private readonly IPreference _preference;
        public MineViewModel(IStorageItemPicker storage, IMessageService notify, IPreference preference)
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
            var folders = await _storage.OpenPickerAsync(new PickerOptions(PickerType.Folder)
            {
                Title = "DownloadPath"
            });

            if (folders == null || folders.Count == 0) return;

            DownloadPath.Value = folders[0].Uri.AbsolutePath;
        }

        protected override async void OnLoad()
        {
            RPC = new ChangeValue<string>(await _preference.GetAsync(Preferences.RPC, ""));
            DownloadPath = new ChangeValue<string>(await _preference.GetAsync(Preferences.DownloadPath, ""));
            SearchTaskCount = new ChangeValue<int>(await _preference.GetAsync(Preferences.SearchTaskCount, 3));
        }

        private async Task SaveAsync()
        {
            try
            {
                if(RPC.IsChange)
                    await _preference.SetAsync(Preferences.RPC, RPC.Value);

                if(DownloadPath.IsChange)
                    await _preference.SetAsync(Preferences.DownloadPath, DownloadPath.Value);

                if (SearchTaskCount.IsChange)
                    await _preference.SetAsync(Preferences.SearchTaskCount, SearchTaskCount.Value);

                _notify.ShowSuccess();
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }
    }
}
