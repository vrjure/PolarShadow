using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PolarShadow.Handlers;
using PolarShadow.ViewModels;

namespace PolarShadow.Views;

public partial class TopLayoutView : UserControl
{
    public TopLayoutView()
    {
        InitializeComponent();
    }

    public TopLayoutView(TopLayoutViewModel vm, IWebViewRequestHandler webViewHandler):this()
    {
        this.DataContext = vm;
        webViewHandler.SetContainer(webViewContainer);
    }
}