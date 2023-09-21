using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.ViewModels;
using System;

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
}
