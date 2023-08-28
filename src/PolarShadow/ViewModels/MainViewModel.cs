using CommunityToolkit.Mvvm.Input;
using PolarShadow.Models;
using PolarShadow.Navigations;
using PolarShadow.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PolarShadow.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _nav;
    public MainViewModel(INavigationService nav)
    {
        _nav = nav;
    }
    public ObservableCollection<MenuIconItem> MenuItems => new ObservableCollection<MenuIconItem>
    {
        new(){ Name = "main", Icon = "\uEE26", ViewType = typeof(BookshelfView)},
        new(){ Name = "discover", Icon = "\uEBC1"},
        new(){ Name = "source", Icon = "\uEAE4", ViewType = typeof(BookSourceView)},
        new(){ Name = "user", Icon="\uF25F"}
    };

    private ICommand _menuClickedCommand;
    public ICommand MenuClickedCommand => _menuClickedCommand ??= new RelayCommand<MenuIconItem>(item =>
    {
        if (item == null || item.ViewType == null) return;
        _nav.Navigate("menuContent", item.ViewType, null);
    });
}
