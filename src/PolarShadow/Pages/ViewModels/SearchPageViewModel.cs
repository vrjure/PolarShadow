using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Pages.ViewModels
{
    public partial class SearchPageViewModel : ObservableObject
    {
        private readonly ILogger _logger;
        private readonly IPolarShadow _polarShadow;
        private string _lastQuery = null;
        private ISearcHandler _seacHandler = null;
        public SearchPageViewModel(IPolarShadow polarShadow, ILogger<SearchPageViewModel> logger)
        {
            _polarShadow = polarShadow;
            _logger = logger;
        }

        [RelayCommand]
        public async Task Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                SearchResult = null;
                return;
            }

            if (!query.Equals(_lastQuery))
            {
                _lastQuery = query;
                _seacHandler = _polarShadow.BuildSearchHandler(new SearchVideoFilter(1, 10, _lastQuery));
            }

            _seacHandler.Reset();
            var result = await _seacHandler.SearchNextAsync();
            SearchResult = new ObservableCollection<VideoSummary>(result);
        }

        [ObservableProperty]
        private ObservableCollection<VideoSummary> searchResult;

        [RelayCommand]
        public async Task SearchNextAsync()
        {
            if (_seacHandler == null)
            {
                return;
            }

            var result = await _seacHandler.SearchNextAsync();
            if (result != null)
            {
                foreach (var item in result)
                {
                    SearchResult.Add(item);
                }
            }
        }

        [RelayCommand]
        public async Task DataSelectChanged(VideoSummary summary)
        {
            if (summary == null)
            {
                return;
            }
            await Shell.Current.GoToAsync(nameof(VideoDetailPage), new Dictionary<string, object>
            {
                { VideoDetailViewModel.Key_VideoSummary, summary }
            });
        }

    }
}
