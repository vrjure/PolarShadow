using PolarShadow.Pages.ViewModels;

namespace PolarShadow.Pages;

public partial class WebBrowserPage : ContentPage, IDisposable
{
	public WebBrowserPage(WebBrowserPageViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}

    public void Dispose()
    {
       
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        (this.BindingContext as WebBrowserPageViewModel).WebViewSource = "";
    }
}