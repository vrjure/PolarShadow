

namespace PolarShadow
{
    public partial class MainPage : ContentPage
    {
        public MainPage(IWebViewRequestHandler webViewHander)
        {
            InitializeComponent();
            webViewHander.SetContainer(webViewContainer);
        }
    }
}