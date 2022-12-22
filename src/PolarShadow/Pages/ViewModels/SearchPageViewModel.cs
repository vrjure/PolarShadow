using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private readonly IPolarShadow _polarShadow;
        private string _lastQuery = null;
        private ISearcHandler _seacHandler = null;
        public SearchPageViewModel(IPolarShadow polarShadow)
        {
            _polarShadow = polarShadow;
        }

        [RelayCommand]
        public async void Search(string query)
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

    }
}
