using PolarShadow.Core;
using PolarShadow.Pages.ViewModels;

namespace PolarShadow;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
