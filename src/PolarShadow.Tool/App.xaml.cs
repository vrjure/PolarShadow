using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Core;
using PolarShadow.Tool.Pages;
using PolarShadow.Tool.Pages.ViewModels;
using PolarShadow.Videos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PolarShadow.Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }
        public new static App Current => (App)Application.Current;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            MainWindow = this.Services.GetRequiredService<MainWindow>();
            InitializeWindow();
            MainWindow.Show();
        }

        private void InitializeWindow()
        {
            if (MainWindow.DataContext is IReferenceUI ui)
            {
                ui.UI = MainWindow;
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            ConfigureUI(services);
            ConfigurePolarShadow(services);
        }

        private void ConfigureUI(IServiceCollection services)
        {
            ConfigureWindow(services);
            ConfigurePage(services);
        }

        private void ConfigureWindow(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>().AddSingleton<MainWindowViewModel>();
        }

        private void ConfigurePage(IServiceCollection services)
        {
            services.AddTransient<MainPage>().AddTransient<MainPageViewModel>();
        }

        private void ConfigurePolarShadow(IServiceCollection service)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var builder = new PolarShadowBuilder();
            builder.ConfigureDefault()
                .AddWebAnalysisItem();
            service.AddSingleton(builder.Build());
        }
    }
}
