using PolarShadow.Pages.ViewModels;

namespace PolarShadow.Pages;

public partial class TestPage : ContentPage
{
	public TestPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync(nameof(WebBrowserPage), new Dictionary<string, object>
		{
			{ WebBrowserPageViewModel.Key_url, "https://www.yhdmp.cc" }
		});
    }
}