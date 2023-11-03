using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Models;
using PolarShadow.Navigations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PolarShadow.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public static string NavigationName = "MainViewContent";
    private readonly INavigationService _nav;
    private readonly INotificationManager _notify;
    public MainViewModel(INavigationService nav, INotificationManager notify)
    {
        _nav = nav;
        _notify = notify;
    }
    public IEnumerable<MenuIconItem> MenuItems => new List<MenuIconItem>
    {
        new(){ Name = "main", Icon = "\uEE26", VMType = typeof(BookshelfViewModel)},
        new(){ Name = "discover", Icon = "\uEBC1", VMType = typeof(DiscoverViewModel)},
        new(){ Name = "source", Icon = "\uEDC6", VMType = typeof(BookSourceViewModel)},
        new(){ Name = "user", Icon="\uF25F", VMType = typeof(MineViewModel)},
#if DEBUG
        new(){ Name = "test", Icon="\uED3E", VMType = typeof(TestViewModel)}
#endif
    };

    private ICommand _menuClickedCommand;
    public ICommand MenuClickedCommand => _menuClickedCommand ??= new RelayCommand<MenuIconItem>(item =>
    {
        if (item == null || item.VMType == null) return;
        try
        {
            _nav.Navigate(NavigationName, item.VMType);

        }
        catch (Exception ex)
        {
            _notify.Show(ex);
        }
    });
}
