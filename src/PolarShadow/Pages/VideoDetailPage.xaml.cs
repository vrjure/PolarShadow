using PolarShadow.Pages.ViewModels;

namespace PolarShadow.Pages;

public partial class VideoDetailPage : ContentPage
{
	public VideoDetailPage(VideoDetailViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}