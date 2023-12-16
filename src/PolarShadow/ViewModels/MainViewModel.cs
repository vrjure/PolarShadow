using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Models;
using PolarShadow.Navigations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        new(){ Name = "test", Icon="\uED3E", VMType = typeof(VideoPlayerViewModel)}
#endif
    };

    private MenuIconItem _selectedValue;
    public MenuIconItem SelectedValue
    {
        get => _selectedValue;
        set
        {
            if (SetProperty(ref _selectedValue, value))
            {
                ToPage(_selectedValue);
            }
        }
    }

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

    protected override void OnSelectionChanged(SelectionModelSelectionChangedEventArgs e)
    {
        if (e.SelectedItems.Count > 0)
        {
            ToPage(e.SelectedItems.First() as MenuIconItem);
        }
    }

    private void ToPage(MenuIconItem item)
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
    }
}
