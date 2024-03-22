using Avalonia.Controls;
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
        new(){ Name = "main", Icon = FindResource<string>("home"), VMType = typeof(BookshelfViewModel)},
        new(){ Name = "discover", Icon = FindResource<string>("discover"), VMType = typeof(DiscoverViewModel)},
        new(){ Name = "source", Icon = FindResource<string>("source"), VMType = typeof(BookSourceViewModel)},
        new(){ Name = "user", Icon = FindResource<string>("user"), VMType = typeof(MineViewModel)},
//#if DEBUG
//        new(){ Name = "test", Icon=FindResource<string>("flask"), VMType = typeof(VideoPlayerViewModel)}
//#endif
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
