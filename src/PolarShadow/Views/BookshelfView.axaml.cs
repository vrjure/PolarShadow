using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.ViewModels;
using System;

namespace PolarShadow.Views;

public partial class BookshelfView : UserControl
{
    public BookshelfView()
    {
        InitializeComponent();
        DataContext = App.Service.GetRequiredService<BookshelfViewModel>();
    }
}