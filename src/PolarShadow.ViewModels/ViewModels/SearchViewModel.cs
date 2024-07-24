using CommunityToolkit.Mvvm.Input;
using PolarShadow.Essentials;
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
using PolarShadow.Notification;

namespace PolarShadow.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly IPolarShadow _polar;
        private readonly IMessageService _notify;
        private readonly INavigationService _nav;
        private readonly IBufferCache _bufferCache;
        private readonly IPreference _preference;

        private ISearchHandler<Resource> _searcHandler;
        public SearchViewModel(IPolarShadow polar, IMessageService notify, INavigationService nav, IBufferCache bufferCache, IPreference preference)
        {
            _polar = polar;
            _notify = notify;
            _nav = nav;
            _bufferCache = bufferCache;
            _preference = preference;
        }

        public IBufferCache ImageCache => _bufferCache;

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(_searchText, value, changed =>
                {
                    _searchText = value;
                });
            }
        }

        private ObservableCollection<Resource> _originResult;
        private ObservableCollection<Resource> _searchResult;
        public ObservableCollection<Resource> SearchResult
        {
            get => _searchResult;
            set => SetProperty(ref _searchResult, value);
        }

        private Resource _selectedValue;
        public Resource SelectedValue
        {
            get => _selectedValue;
            set
            {
                if(SetProperty(ref _selectedValue, value))
                {
                    OnSelected(SelectedValue);
                }
            }
        }

        private ObservableCollection<string> _availableSites;
        public ObservableCollection<string> AvailableSites
        {
            get => _availableSites;
            set => SetProperty(ref _availableSites, value);
        }

        private IReadOnlyList<int> _selectedSiteFilters;
        public IReadOnlyList<int> SelectedSiteFilters
        {
            get => _selectedSiteFilters;
            set
            {
                var newVal = value;
                var oldVal = _selectedSiteFilters;
                var isChanged = false;

                if (newVal?.Count != oldVal?.Count)
                {
                    isChanged = true;
                }
                else if (newVal != null && oldVal != null && newVal.Except(oldVal).Count() > 0)
                {
                    isChanged = true;
                }

                _selectedSiteFilters = newVal;
                if (isChanged)
                {
                    OnSiteFilterChanged();
                }
            }
        }

        private bool _showLoadMore = false;
        public bool ShowLoadMore
        {
            get => _showLoadMore;
            set => SetProperty(ref _showLoadMore, value);
        }

        private IAsyncRelayCommand _searchCommand;
        public IAsyncRelayCommand SearchCommand => _searchCommand ??= new AsyncRelayCommand(Search);

        private IAsyncRelayCommand _loadMoreCommand;
        public IAsyncRelayCommand LoadMoreCommand => _loadMoreCommand ??= new AsyncRelayCommand(LoadMore);


        protected override void OnLoad()
        {
            AvailableSites = new ObservableCollection<string>(_polar.GetVideoSites().Where(f => f.HasRequest(Requests.Search)).Select(f => f.Name));
        }

        private void OnSiteFilterChanged()
        {
            var sites = GetFilterSites();
            if (sites != null)
            {
                if (_originResult?.Count > 0)
                {
                    SearchResult = new ObservableCollection<Resource>(_originResult.Where(f => sites.Any(o => o.Name == f.Site)));
                }
            }
            else
            {
                SearchResult = _originResult;
            }

            ShowLoadMore = false;
        }

        private IEnumerable<ISite> GetFilterSites()
        {
            if (SelectedSiteFilters?.Count > 0)
            {
                HashSet<string> siteNames = new HashSet<string>();
                foreach (var item in SelectedSiteFilters)
                {
                    siteNames.Add(AvailableSites[item]);
                }
                return _polar.GetVideoSites().Where(f => siteNames.Contains(f.Name));
            }

            return null;
        }

        private async Task Search()
        {
            _searchResult?.Clear();
            if (string.IsNullOrEmpty(SearchText))
            {
                IsLoading = false;
                ShowLoadMore = false;
                return;
            }

            var taskCount = await _preference.GetAsync(Preferences.SearchTaskCount, 3);
            var sites = GetFilterSites();
            if (sites != null)
            {               
                _searcHandler = _polar.CreateSearchHandler<Resource>(SearchText, sites, taskCount);
            }
            else
            {
                _searcHandler = _polar.CreateSearchHandler<Resource>(SearchText, taskCount);
            }

            await LoadMore();
        }

        private async Task LoadMore()
        {
            IsLoading = true;
            ShowLoadMore = false;
            try
            {
                var result = await _searcHandler.SearchNextAsync(Cancellation.Token);

                if (result == null || result.Count == 0)
                {
                    ShowLoadMore = false;
                    return;
                }

                if (SearchResult == null)
                {    
                     SearchResult = new ObservableCollection<Resource>(result);
                }
                else
                {
                    foreach (var item in result)
                    {
                        SearchResult.Add(item);
                    }
                }

                _originResult = SearchResult;

                ShowLoadMore = true;
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _notify.Show(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnSelected(Resource selected)
        {
            if (selected == null) return;
            NavigateToDetail(selected);
        }

        private void NavigateToDetail(Resource searchValue)
        {
            if (searchValue == null)
            {
                return;
            }

            _nav.Navigate<DetailViewModel>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>
            {
                {nameof(DetailViewModel.Param_Link), searchValue }
            }, true);
        }
    }
}
