using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Essentials;
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
            Sites = new ObservableCollection<ISite>(_polar.GetVideoSites());
        }

        private void ToSiteDetail(ISite site)
        {
            if (site == null)
            {
                return;
            }

            _nav.Navigate<BookSourceDetailViewModel>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>()
            {
                {nameof(BookSourceDetailViewModel.Site), site}
            }, true);
        }

        protected override void OnSelectionChanged(SelectionModelSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count > 0)
            {
                ToSiteDetail(e.SelectedItems[0] as ISite);
                SelectionModel.Deselect(e.SelectedIndexes[0]);
            }
        }
    }
}
