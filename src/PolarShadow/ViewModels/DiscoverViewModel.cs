using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
using PolarShadow.Core;
using PolarShadow.Navigations;
using PolarShadow.Resources;
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

        private ISite _selectValue;
        public ISite SelectValue
        {
            get => _selectValue;
            set
            {
                if (SetProperty(ref _selectValue, value))
                {
                    SiteSelectChanged();
                }
            }
        }

        private ISelectionModel _selection;
        public ISelectionModel Selection
        {
            get => _selection;
            set => SetProperty(ref _selection, value);
        }

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

        private void SiteSelectChanged()
        {
            if (SelectValue == null)
            {
                return;
            }

            _nav.Navigate<DiscoverDetailView>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>
            {
                { nameof(DiscoverDetailViewModel.Param_Site), SelectValue }
            }, true);

            Selection?.Clear();
            SelectValue = null;
        }
    }
}
