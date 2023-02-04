using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PolarShadow.Tool.Pages.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IPolarShadow _polarShadow;
        public MainPageViewModel(IPolarShadow polarShadow)
        {
            _polarShadow = polarShadow;
            var sites = _polarShadow.GetSites();
            foreach (var item in sites)
            {
                this.Sites.Add(new ListItem(item.Name, item));
            }
        }

        public ObservableCollection<ListItem> Sites { get; } = new();
        public ObservableCollection<string> AbilitiyNames { get; } = new();

        [ObservableProperty]
        private string input;
        [ObservableProperty]
        private string output;

        private IPolarShadowSite _selectSite;
        [RelayCommand]
        public void SiteSelectionChanged(SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as ListItem;

                AbilitiyNames.Clear();
                _selectSite = item.Value as IPolarShadowSite;
                foreach (var ability in _selectSite.EnumerableAbilities())
                {
                    AbilitiyNames.Add(ability.Name);
                }
            }
        }

        [RelayCommand]
        public void AbilitySelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var abilityName = e.AddedItems[0].ToString();
            }
        }

        [RelayCommand]
        public void TestAbility()
        {

        }
    }
}
