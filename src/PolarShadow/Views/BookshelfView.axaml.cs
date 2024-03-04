using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Transformation;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.ViewModels;
using System;

namespace PolarShadow.Views;

public partial class BookshelfView : UserControl
{
    public BookshelfView()
    {
        InitializeComponent();
    }

    public BookshelfView(BookshelfViewModel vm) : this()
    {
        this.DataContext = vm;
    }
}