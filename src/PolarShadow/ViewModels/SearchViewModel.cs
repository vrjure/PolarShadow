using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Cache;
using PolarShadow.Core;
using PolarShadow.Navigations;
using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly IPolarShadow _polar;
        private readonly INotificationManager _notify;
        private readonly INavigationService _nav;
        private readonly IBufferCache _bufferCache;
        public SearchViewModel(IPolarShadow polar, INotificationManager notify, INavigationService nav, IBufferCache bufferCache)
        {
            _polar = polar;
            _notify = notify;
            _nav = nav;
            _bufferCache = bufferCache;
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

        private ObservableCollection<Resource> _searchResult;
        public ObservableCollection<Resource> SearchResult
        {
            get => _searchResult;
            set => SetProperty(ref _searchResult, value);
        }

        private bool _showLoadMore = false;
        public bool ShowLoadMore
        {
            get => _showLoadMore;
            set => SetProperty(ref _showLoadMore, value);
        }

        private IAsyncRelayCommand _searchCommand;
        public IAsyncRelayCommand SearchCommand => _searchCommand ??= new AsyncRelayCommand(Search);

        private IRelayCommand _clearCommand;
        public IRelayCommand ClearCommand => _clearCommand ??= new RelayCommand(() => { });

        private IAsyncRelayCommand _loadMoreCommand;
        public IAsyncRelayCommand LoadMoreCommand => _loadMoreCommand ??= new AsyncRelayCommand(LoadMore);


        private ISearchHandler<Resource> _searcHandler;
        private async Task Search()
        {
            _searchResult?.Clear();
            if (string.IsNullOrEmpty(SearchText))
            {
                return;
            }
            _searcHandler = _polar.CreateSearchHander<Resource>(SearchText);

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

        protected override void OnSelectionChanged(SelectionModelSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count> 0)
            {
                NavigateToDetail(e.SelectedItems.First() as Resource);
                SelectionModel.Deselect(e.SelectedIndexes.First());
            }
        }
    }
}
