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
        webView.Navigated -= WebView_Navigated;
        webView.Navigated += WebView_Navigated;
        webView.Source = "https://www.yhdmp.cc";
    }

    private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        var result = await webView.EvaluateJavaScriptAsync($"document.documentElement;");
        text.Text = result;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        webView.Navigated -= WebView_Navigated;
        (this.BindingContext as WebBrowserPageViewModel).WebViewSource = "";

    }
}