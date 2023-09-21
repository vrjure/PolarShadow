using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PolarShadow.ViewModels;

namespace PolarShadow.Views;

public partial class BookSourceDetailView : UserControl
{
    public BookSourceDetailView()
    {
        InitializeComponent();
    }

    public BookSourceDetailView(BookSourceDetailViewModel vm) : this()
    {
        this.DataContext = vm;
    }
}