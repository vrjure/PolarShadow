using CommunityToolkit.Mvvm.Input;
using PolarShadow.Essentials;
using PolarShadow.Models;
using PolarShadow.Navigations;
using PolarShadow.Notification;
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
    private readonly IMessageService _notify;
    private readonly IResourceService _resourceService;
    public MainViewModel(INavigationService nav, IMessageService notify, IResourceService resourceService)
    {
        _nav = nav;
        _notify = notify;
        _resourceService = resourceService;
    }
    public IEnumerable<MenuIconItem> MenuItems => new List<MenuIconItem>
    {
        new(){ Name = "main", Icon = _resourceService.FindResource<string>("home"), VMType = typeof(BookshelfViewModel)},
        new(){ Name = "discover", Icon = _resourceService.FindResource<string>("discover"), VMType = typeof(DiscoverViewModel)},
        new(){ Name = "source", Icon = _resourceService.FindResource<string>("source"), VMType = typeof(BookSourceViewModel)},
        new(){ Name = "user", Icon = _resourceService.FindResource<string>("user"), VMType = typeof(MineViewModel)},
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
