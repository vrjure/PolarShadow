using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Navigations;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class DiscoverViewModel : ViewModelBase
    {
        private readonly IPolarShadow _polar;
        private readonly INotificationManager _notify;
        private readonly INavigationService _nav;

        public DiscoverViewModel(IPolarShadow polar, INotificationManager notify, INavigationService nav)
        {
            _polar = polar;
            _notify = notify;
            _nav = nav;
        }

        private ObservableCollection<ISite> _sites;
        public ObservableCollection<ISite> Sites
        { 
            get => _sites;
            set => SetProperty(ref _sites, value);
        }

        private IRelayCommand _siteSelectedCommand;
        public IRelayCommand SiteSelectedCommand => _siteSelectedCommand ??= new RelayCommand<ISite>(SiteSelectChanged);

        protected override void OnLoad()
        {
            try
            {
                var sites = _polar.GetSites(f => f.HasRequest(Requests.Categories));
                Sites = new ObservableCollection<ISite>(sites);
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
        }

        private void SiteSelectChanged(ISite site)
        {
            if (site == null)
            {
                return;
            }

            _nav.Navigate<DiscoverDetailViewModel>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>
            {
                { nameof(DiscoverDetailViewModel.Param_Site), site }
            }, true);
        }
    }
}
