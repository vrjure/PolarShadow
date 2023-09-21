using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PolarShadow.ViewModels;

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
}