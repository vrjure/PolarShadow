using PolarShadow.Navigations;
using PolarShadow.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _nav;
        public MainWindowViewModel(INavigationService nav)
        {
            _nav = nav;
        }

        public static string NavigationName => "MenuContent";

    }
}
