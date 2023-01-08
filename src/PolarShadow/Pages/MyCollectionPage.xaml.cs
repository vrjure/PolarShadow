using PolarShadow.Pages.ViewModels;

namespace PolarShadow.Pages;

public partial class MyCollectionPage : ContentPage
{
	private readonly MyCollectionViewModel _vm;
	public MyCollectionPage(MyCollectionViewModel vm)
	{
		BindingContext = _vm = vm;
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
		await _vm.InitializeAsync();
		cv.SelectedItem= null;
    }
}