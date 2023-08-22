using PolarShadow.Views;

namespace PolarShadow.Pages;

public partial class PlayerPage : ContentPage
{
    private Uri _uri;
	public PlayerPage()
	{
		InitializeComponent();
	}

    public PlayerPage(Uri uri) : base()
    {
        _uri = uri;
    }
}