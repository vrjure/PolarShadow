using CommunityToolkit.Mvvm.Input;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Essentials;
using PolarShadow.Models;
using PolarShadow.Storage;
using CommunityToolkit.Mvvm.DependencyInjection;
using PolarShadow.Services.Http;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Core;

namespace PolarShadow.ViewModels
{
    public class MineViewModel : ViewModelBase
    {
        private readonly IStorageItemPicker _storage;
        private readonly IMessageService _notify;
        private readonly IDbPreferenceService _dbPreference;
        private readonly IServiceProvider _service;
        private readonly IPolarShadow _polar;
        public MineViewModel(IStorageItemPicker storage, IMessageService notify, IDbPreferenceService dbPreference, IServiceProvider service, IPolarShadow polar)
        {
            _storage = storage;
            _notify = notify;
            _dbPreference = dbPreference;
            _service = service;
            _polar = polar;
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

        private ChangeValue<string> _apiAddress;
        public ChangeValue<string> ApiAddress
        {
            get => _apiAddress;
            set => SetProperty(ref _apiAddress, value);
        }

        private ChangeValue<bool> _apiEnable;
        public ChangeValue<bool> ApiEnable
        {
            get => _apiEnable;
            set => SetProperty(ref _apiEnable, value);
        }

        private ChangeValue<string> _userName;
        public ChangeValue<string> UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private ChangeValue<string> _password;
        public ChangeValue<string> Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private IAsyncRelayCommand _pickDownloadPathCommand;
        public IAsyncRelayCommand PickDownloadPathCommand => _pickDownloadPathCommand ??= new AsyncRelayCommand(PickDownloadFolder);

        private IAsyncRelayCommand _saveCommand;
        public IAsyncRelayCommand SaveCommand => _saveCommand ??= new AsyncRelayCommand(SaveAsync);

        public IAsyncRelayCommand _uploadCloudCommand;
        public IAsyncRelayCommand UploadCloudCommand => _uploadCloudCommand ??= new AsyncRelayCommand(UploadCloudAsync);

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
            try
            {
                ApiAddress = new ChangeValue<string>(await _dbPreference.GetAsync(Preferences.ServerAddress, ""));
                ApiEnable = new ChangeValue<bool>(await _dbPreference.GetAsync(Preferences.ApiEnable, false));
                RPC = new ChangeValue<string>(await _dbPreference.GetAsync(Preferences.RPC, ""));
                DownloadPath = new ChangeValue<string>(await _dbPreference.GetAsync(Preferences.DownloadPath, ""));
                SearchTaskCount = new ChangeValue<int>(await _dbPreference.GetAsync(Preferences.SearchTaskCount, 3));
                UserName = new ChangeValue<string>(await _dbPreference.GetAsync(Preferences.UserName, ""));
                Password = new ChangeValue<string>(await _dbPreference.GetAsync(Preferences.Password, ""));
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }

        private async Task SaveAsync()
        {
            try
            {
                if (RPC.IsChange)
                {
                    await _dbPreference.SetAsync(Preferences.RPC, RPC.Value);
                    RPC.Reset();
                }

                if (DownloadPath.IsChange)
                {
                    await _dbPreference.SetAsync(Preferences.DownloadPath, DownloadPath.Value);
                    DownloadPath.Reset();
                }

                if (SearchTaskCount.IsChange)
                {
                    await _dbPreference.SetAsync(Preferences.SearchTaskCount, SearchTaskCount.Value);
                    SearchTaskCount.Reset();
                }

                if (ApiAddress.IsChange)
                {
                    await _dbPreference.SetAsync(Preferences.ServerAddress, ApiAddress.Value);
                    ApiAddress.Reset();
                }

                if (UserName.IsChange)
                {
                    await _dbPreference.SetAsync(Preferences.UserName, UserName.Value);
                    UserName.Reset();
                }

                if (Password.IsChange)
                {
                    await _dbPreference.SetAsync(Preferences.Password, Password.Value);
                    Password.Reset();
                }

                if (ApiEnable.IsChange)
                {
                    await _dbPreference.SetAsync(Preferences.ApiEnable, ApiEnable.Value);
                    ApiEnable.Reset();

                }

                if (ApiEnable.Value)
                {
                    try
                    {
                        var serverService = _service.GetRequiredService<IServerService>();
                        await serverService.GetServerTimeAsync();

                        await UpdateSourceAsync();
                    }
                    catch (Exception ex)
                    {
                        ApiEnable.Reset(false);
                        await _dbPreference.SetAsync(Preferences.ApiEnable, ApiEnable.Value);
                        _notify.Show(ex);
                        return;
                    }
                }

                _notify.ShowSuccess();
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
        }

        private async Task UploadCloudAsync()
        {
            try
            {
                var resourceSync = _service.GetRequiredService<ISyncService<ResourceModel>>();
                var historySync = _service.GetRequiredService<ISyncService<HistoryModel>>();
                var sourceSync = _service.GetRequiredService<ISyncService<SourceModel>>();

                await resourceSync.SyncAsync();
                await historySync.SyncAsync();
                await sourceSync.SyncAsync();

                await UpdateSourceAsync();

                _notify.ShowSuccess();
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
        }
        private async Task UpdateSourceAsync()
        {
            var sourceService = _service.GetRequiredService<ISourceService>();
            var source = await sourceService.GetSouuceAsync();
            if (source != null && !string.IsNullOrEmpty(source.Data))
            {
                _polar.Load(new JsonStringSource() { Json = source.Data }, true);
            }
        }
    }
}
