using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Navigations;
using PolarShadow.Resources;
using PolarShadow.Services;
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
        private readonly INotificationManager _notification;
        private readonly INavigationService _nav;
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        public BookSourceViewModel(IPolarShadow polar, IStorageService storage, INotificationManager notification, INavigationService nav, IDbContextFactory<PolarShadowDbContext> dbFactory)
        {
            _polar = polar;
            _storage = storage;
            _notification = notification;
            _nav = nav;
            _dbFactory = dbFactory;
        }
        private ObservableCollection<ISite> _sites;
        public ObservableCollection<ISite> Sites
        {
            get => _sites;
            set => SetProperty(ref _sites, value);
        }

        private ISite _selectedSite;
        public ISite SelectedSite
        {
            get => _selectedSite;
            set => SetProperty(_selectedSite, value, val =>
            {
                _selectedSite = val;
                OnSiteSelectorChanged();
            });
        }

        private ISelectionModel _selection;
        public ISelectionModel Selection
        {
            get => _selection;
            set => SetProperty(ref _selection, value);
        }

        private IAsyncRelayCommand _importCommand;
        public IAsyncRelayCommand ImportCommand => _importCommand ??= new AsyncRelayCommand(ImportSource);

        protected override void OnLoad()
        {
            Reflesh();
        }

        private async Task ImportSource()
        {
            try
            {
                var file = await _storage.OpenPolarShadowConfigFilePickerAsync();
                if (file == null) return;

                _polar.LoadJsonStreamSource(await file.OpenReadAsync());

                Reflesh();

                await _polar.SaveToAsync(new DbConfigurationSource
                {
                    DbContextFactroy = _dbFactory
                });

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

        private void OnSiteSelectorChanged()
        {
            if (SelectedSite == null)
            {
                return;
            }

            var selectSite = SelectedSite;
            _nav.Navigate<BookSourceDetailViewModel>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>()
            {
                {nameof(BookSourceDetailViewModel.Site), selectSite }
            }, true);

            Selection?.Clear();
            SelectedSite = null;
        }
    }
}
