using CommunityToolkit.Mvvm.ComponentModel;
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
        public SearchViewModel(IPolarShadow polar)
        {
            _polar = polar;
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
        
    }
}
