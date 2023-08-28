﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Navigations;
using System;

namespace PolarShadow.Views;

public partial class MainWindow : Window
{
    private readonly INavigationService _nav;
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(INavigationService nav) : this()
    {
        _nav = nav;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (Design.IsDesignMode) return;
        _nav.Navigate("content", typeof(MainView), default);
    }
}
