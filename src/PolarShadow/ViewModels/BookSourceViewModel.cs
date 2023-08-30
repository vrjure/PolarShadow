﻿using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Navigations;
using PolarShadow.Services;
using PolarShadow.Views;
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
        public BookSourceViewModel(IPolarShadow polar, IStorageService storage, INotificationManager notification, INavigationService nav)
        {
            _polar = polar;
            _storage = storage;
            _notification = notification;
            _nav = nav;
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
                var file = await _storage.OpenPolarShadowConfigFilePickerAsync();
                if (file == null) return;

                _polar.LoadJsonStreamSource(await file.OpenReadAsync());

                Reflesh();

                _polar.Save();

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
            _nav.Navigate<BookSourceDetailView>(MainViewModel.NavigationName, new Dictionary<string, object>()
            {
                {nameof(BookSourceDetailViewModel.Site), _selectedSite }
            });
        }
    }
}