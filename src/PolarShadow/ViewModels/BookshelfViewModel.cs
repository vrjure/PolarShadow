using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Cache;
using PolarShadow.Navigations;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class BookshelfViewModel : ViewModelBase
    {
        private readonly INavigationService _nav;
        private readonly IMineResourceService _mineResourceService;
        private readonly INotificationManager _notify;
        public BookshelfViewModel(INavigationService nav, IBufferCache cache, IMineResourceService mineResourceService, INotificationManager notify)
        {
            _nav = nav;
            Cache = cache;
            _mineResourceService = mineResourceService;
            _notify = notify;
        }

        public IBufferCache Cache { get; }

        private ObservableCollection<ResourceModel> _mineResource;
        public ObservableCollection<ResourceModel> MineResource
        {
            get => _mineResource;
            set => SetProperty(ref _mineResource, value);
        }

        private ResourceModel _selectValue;
        public ResourceModel SelectValue
        {
            get => _selectValue;
            set
            {
                SetProperty(_selectValue, value, changed =>
                {
                    _selectValue = changed;
                    ToDetail();
                });
            }
        }

        private ISelectionModel _selection;
        public ISelectionModel Selection
        {
            get => _selection;
            set => SetProperty(ref _selection, value);
        }

        private IRelayCommand _searchCommand;
        public IRelayCommand SearchCommand => _searchCommand ??= new RelayCommand(() => _nav.Navigate<SearchViewModel>(TopLayoutViewModel.NavigationName, canBack: true));

        protected override async void OnLoad()
        {
            try
            {
                var savedResource = await _mineResourceService.GetRootResourcesAsync();
                MineResource = new ObservableCollection<ResourceModel>(savedResource);
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }

        public void ToDetail()
        {
            if (SelectValue == null)
            {
                return;
            }

            var selected = SelectValue;

            _nav.Navigate<DetailViewModel>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>
            {
                {nameof(DetailViewModel.Param_Link), selected }
            }, true);

            Selection?.Clear();
            SelectValue = null;
        }
        
    }
}
