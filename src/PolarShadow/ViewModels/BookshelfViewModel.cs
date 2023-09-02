using CommunityToolkit.Mvvm.Input;
using PolarShadow.Navigations;
using PolarShadow.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class BookshelfViewModel : ViewModelBase
    {
        private readonly INavigationService _nav;
        public BookshelfViewModel(INavigationService nav)
        {
            _nav = nav;
        }

        private IRelayCommand _searchCommand;
        public IRelayCommand SearchCommand => _searchCommand ??= new RelayCommand(() => _nav.Navigate<SearchView>(TopLayoutViewModel.NavigationName, canBack:true));
    }
}
