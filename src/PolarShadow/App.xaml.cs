namespace PolarShadow
{
    public partial class App : Application
    {
        public App(IServiceProvider service)
        {
            InitializeComponent();

            MainPage = service.GetRequiredService<NavigationPage>();
        }
    }
}