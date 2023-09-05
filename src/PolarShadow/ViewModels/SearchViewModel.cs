using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Videos;
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
        public SearchViewModel(IPolarShadow polar, INotificationManager notify)
        {
            _polar = polar;
            _notify = notify;
        }

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

        private ObservableCollection<VideoSummary> _searchResult;
        public ObservableCollection<VideoSummary> SearchResult
        {
            get => _searchResult;
            set => SetProperty(ref _searchResult, value);
        }

        private bool _showIndicator = false;
        public bool ShowIndicator
        {
            get => _showIndicator;
            set => SetProperty(ref _showIndicator, value);
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


        private IVideoSearcHandler _searcHandler;
        private async Task Search()
        {
            _searchResult?.Clear();
            if (string.IsNullOrEmpty(SearchText))
            {
                return;
            }
            _searcHandler = _polar.CreateVideoSearcHandler(new SearchVideoFilter
            {
                Page = 1,
                PageSize = 10,
                SearchKey = SearchText
            });

            await LoadMore();
        }

        private async Task LoadMore()
        {
            ShowIndicator = true;
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
                    SearchResult = new ObservableCollection<VideoSummary>(result.Data);
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

            ShowIndicator = false;
        }
        
    }
}
