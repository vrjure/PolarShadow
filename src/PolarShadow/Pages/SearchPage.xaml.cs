using PolarShadow.Pages.ViewModels;

namespace PolarShadow.Pages;

public partial class SearchPage : ContentPage
{
	private readonly SearchPageViewModel _vm;
	public SearchPage(SearchPageViewModel vm)
	{
		this.BindingContext = _vm = vm;
		InitializeComponent();
	}

    private async void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
    {
		if (e.ItemIndex == _vm.SearchResult.Count - 1)
		{
			await _vm.SearchNextAsync();
		}
    }
}