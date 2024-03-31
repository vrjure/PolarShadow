using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Essentials;
using PolarShadow.Navigations;
using PolarShadow.Notification;
using PolarShadow.Resources;
using PolarShadow.Services;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class BookSourceViewModel : ViewModelBase
    {
        private readonly IPolarShadow _polar;
        private readonly IStorageItemPicker _storage;
        private readonly IMessageService _notification;
        private readonly INavigationService _nav;
        public BookSourceViewModel(IPolarShadow polar, IStorageItemPicker storage, IMessageService notification, INavigationService nav)
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
                var source = await _storage.OpenPickerAsync(new PickerOptions());
                if (source == null) return;

                //_polar.LoadJsonStreamSource(source);

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
    }
}
