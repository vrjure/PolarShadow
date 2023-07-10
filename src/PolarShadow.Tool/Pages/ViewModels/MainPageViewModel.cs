using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PolarShadow.Tool.Pages.ViewModels
{
    public partial class MainPageViewModel : ObservableObject, IContextStorage
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
        public ObservableCollection<ListItem> Abilities { get; } = new();

        [ObservableProperty]
        private string input;
        [ObservableProperty]
        private string output;

        private ISite _selectSite;
        private string _selectAbility;
        [RelayCommand]
        public void SiteSelectionChanged(SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as ListItem;

                Abilities.Clear();
                _selectSite = item.Value as ISite;
                foreach (var ability in _selectSite.Abilities)
                {
                    Abilities.Add(new ListItem(ability, ability));
                }
            }
        }

        [RelayCommand]
        public void AbilitySelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as ListItem;
                _selectAbility = item.Value.ToString();
            }
        }

        [RelayCommand]
        public async Task TestAbility()
        {
            try
            {

                var result = await _selectSite.ExecuteAsync(_selectAbility, Input);
                using var doc = JsonDocument.Parse(result);

                using var ms = new MemoryStream();
                using var jsonWriter = new Utf8JsonWriter(ms, JsonOption.FormatWriteOption);
                doc.WriteTo(jsonWriter);
                jsonWriter.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                using var sr = new StreamReader(ms);
                Output = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public object Save()
        {
            return Input;
        }

        public void Apply(object context)
        {
            Input = context?.ToString();
        }
    }
}
