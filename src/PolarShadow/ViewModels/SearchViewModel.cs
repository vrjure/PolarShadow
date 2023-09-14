using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Caching.Memory;
using PolarShadow.Cache;
using PolarShadow.Core;
using PolarShadow.Models;
using PolarShadow.Navigations;
using PolarShadow.Resources;
using PolarShadow.Views;
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

        private Resource _selectValue;
        public Resource SelectValue
        {
            get => _selectValue;
            set
            {
                SetProperty(_selectValue, value, changed => 
                {
                    _selectValue = value;
                    NavigateToDetail();
                });
            }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _showLoadMore = false;
        public bool ShowLoadMore
        {
            get => _showLoadMore;
            set => SetProperty(ref _showLoadMore, value);
        }

        private bool _hasData = true;
        public bool HasData
        {
            get => _hasData;
            set => SetProperty(ref _hasData, value);
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
            _searcHandler = _polar.CreateSearchHander<Resource>(new SearchFilter
            {
                Page = 1,
                PageSize = 20,
                SearchKey = SearchText
            });

            await LoadMore();
        }

        private async Task LoadMore()
        {
            IsLoading = true;
            ShowLoadMore = false;
            try
            {
                var result = await _searcHandler.SearchNextAsync();

                if (result == null || result.Data == null || result.Data.Count == 0)
                {
                    ShowLoadMore = false;
                    return;
                }

                if (SearchResult == null)
                {
                    SearchResult = new ObservableCollection<Resource>(result.Data);
                }
                else
                {
                    foreach (var item in result.Data)
                    {
                        SearchResult.Add(item);
                    }
                }
                ShowLoadMore = true;
            }
            catch (Exception ex)
            {
                _notify.Show(ex.Message);
            }

            IsLoading = false;
        }

        private void NavigateToDetail()
        {
            _nav.Navigate<DetailView>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>
            {
                {nameof(DetailViewModel.Param_Link), SelectValue }
            }, true);
        }     
    }
}
