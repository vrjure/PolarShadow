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
    public MainViewModel(INavigationService nav, IMessageService notify)
    {
        _nav = nav;
        _notify = notify;
    }
    private ObservableCollection<MenuIconItem> _menuItems;
    public ObservableCollection<MenuIconItem> MenuItems
    {
        get => _menuItems;
        set => SetProperty(ref _menuItems, value);
    }
    

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
