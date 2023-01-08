using PolarShadow.Pages.ViewModels;

namespace PolarShadow.Pages;

public partial class ConfigurePage : ContentPage
{
	public ConfigurePage(ConfigPageViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}