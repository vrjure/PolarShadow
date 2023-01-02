
using CommunityToolkit.Maui.Views;
using PolarShadow.Core;

namespace PolarShadow.Pages.Controls;

public partial class HtmlAnalysisSelector : Popup
{
	public HtmlAnalysisSelector(ICollection<HtmlAnalysisSource> sources)
	{
		InitializeComponent();

		dataList.ItemsSource = sources;
	}
}