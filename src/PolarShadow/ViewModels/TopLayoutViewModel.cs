using PolarShadow.Navigations;
using PolarShadow.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class TopLayoutViewModel : ViewModelBase
    {
        public static string NavigationName = "TopLayoutContent";
        public static string RightTitleBarContainer = "RightTitleBarContianer";
        public static string CenterTitleBarContainer = "CenterTItleBarContainer";

        private readonly INavigationService _nav;
        public TopLayoutViewModel(INavigationService nav)
        {
            _nav = nav;
        }

        public override void OnLoad()
        {
            _nav.Navigate<MainView>(NavigationName);
        }
    }
}
