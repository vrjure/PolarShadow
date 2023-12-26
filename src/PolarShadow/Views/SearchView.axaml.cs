using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PolarShadow.ViewModels;
using System.Linq;

namespace PolarShadow.Views;

public partial class SearchView : UserControl
{
    public SearchView()
    {
        InitializeComponent();
    }

    public SearchView(SearchViewModel vm):this()
    {
        this.DataContext = vm;
    }

    private SearchViewModel VM => this.DataContext as SearchViewModel;

    private void Flyout_Opened(object sender, System.EventArgs e)
    {
        if (VM.SelectedSiteFilters?.Count > 0)
        {
            foreach (var item in VM.SelectedSiteFilters)
            {
                VM.SiteFilterSelection.Select(item);
            }
        }
    }

    private void Flyout_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        VM.SelectedSiteFilters = VM.SiteFilterSelection.SelectedIndexes.ToList();
    }
}