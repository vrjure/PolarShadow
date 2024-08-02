using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Essentials;
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
        private readonly IMessageService _notify;
        private readonly INavigationService _nav;

        public DiscoverViewModel(IPolarShadow polar, IMessageService notify, INavigationService nav)
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

        private ISite _selectedSite;
        public ISite SelectedSite
        {
            get => _selectedSite;
            set 
            {
                if(SetProperty(ref _selectedSite, value))
                {
                    SiteSelectChanged(_selectedSite);
                }
            } 
        }

        protected override void OnLoad()
        {
            try
            {
                SelectedSite = null;
                var sites = _polar.GetVideoSites(f => f.HasRequest(Requests.Categories));
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
