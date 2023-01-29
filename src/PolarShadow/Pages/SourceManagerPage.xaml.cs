using PolarShadow.Pages.ViewModels;

namespace PolarShadow.Pages;

public partial class SourceManagerPage : ContentPage
{
	public SourceManagerPage(SourceManagerViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}