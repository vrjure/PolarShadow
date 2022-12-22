using PolarShadow.Pages.ViewModels;

namespace PolarShadow.Pages;

public partial class SearchPage : ContentPage
{
	public SearchPage(SearchPageViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}