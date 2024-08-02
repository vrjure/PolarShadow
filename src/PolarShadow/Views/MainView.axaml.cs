using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Models;
using PolarShadow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PolarShadow.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public MainView(MainViewModel vm) : this()
    {
        this.DataContext = vm;
    }
    public MainViewModel VM => DataContext as MainViewModel;

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (VM.MenuItems == null)
        {
            VM.MenuItems = new ObservableCollection<MenuIconItem>
            {
                new() { Name = "main", Icon = (string)this.FindResource("home"), VMType = typeof(BookshelfViewModel) },
                new() { Name = "discover", Icon = (string)this.FindResource("discover"), VMType = typeof(DiscoverViewModel) },
                new() { Name = "source", Icon = (string)this.FindResource("source"), VMType = typeof(BookSourceViewModel) },
                new() { Name = "user", Icon = (string)this.FindResource("user"), VMType = typeof(MineViewModel) },
            };

            VM.SelectedValue = VM.MenuItems.FirstOrDefault();
        }
    }
}
