using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Core;
using PolarShadow.Dispatcher;
using PolarShadow.Essentials;
using PolarShadow.Navigations;
using PolarShadow.Notification;
using PolarShadow.Resources;
using PolarShadow.Storage;
using PolarShadow.StoragePicker;
using PolarShadow.ViewModels;
using PolarShadow.WebView;
using PolarShadow.WPF.Views;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace PolarShadow.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            RegisterView(services);
            RegisterUtilities(services);
            RegisterDatabase(services);
            RegisterPolarShadow(services);
            Ioc.Default.ConfigureServices(services.BuildServiceProvider());

            var mainWindow = Ioc.Default.GetService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);

        }

        private void RegisterView(IServiceCollection service)
        {
            service.RegisterSingletonViewWithModel<MainWindow, MainWindowViewModel>();
            service.RegisterSingletonViewWithModel<TopLayoutView, TopLayoutViewModel>();
            service.RegisterTransientViewWithModel<MainView, MainViewModel>();
            service.RegisterTransientViewWithModel<BookshelfView, BookshelfViewModel>();
            service.RegisterTransientViewWithModel<BookSourceView, BookSourceViewModel>();
            service.RegisterTransientViewWithModel<BookSourceDetailView, BookSourceDetailViewModel>();
            service.RegisterTransientViewWithModel<SearchView, SearchViewModel>();
            service.RegisterTransientViewWithModel<DetailView, DetailViewModel>();
            service.RegisterTransientViewWithModel<DiscoverView, DiscoverViewModel>();
            service.RegisterTransientViewWithModel<DiscoverDetailView, DiscoverDetailViewModel>();
            service.RegisterTransientViewWithModel<MineView, MineViewModel>();
        }

        private void RegisterUtilities(IServiceCollection service)
        {
            service.RegisterCache(new FileCacheOptions { CacheFolder = PolarShadowApp.CacheFolder});
            service.RegisterDbPreference();
            service.RegisterVLC();
            service.AddSingleton<INavigationService, NavigationService>();
            service.AddSingleton<IMessageService, NotificationContainer>();
            service.AddSingleton<IDispatcherUI, DispatcherUI>();
            service.AddSingleton<IStorageItemPicker, StorageItemPicker>();
        }

        private void RegisterDatabase(IServiceCollection service)
        {
            service.AddDbContextFactory<PolarShadowDbContext>(op =>
            {
                op.UseSqlite($"Data Source={PolarShadowApp.DbFile}", op => op.MigrationsAssembly(typeof(App).Assembly.FullName));
            });
            service.RegisterStorageService();
        }

        private void RegisterPolarShadow(IServiceCollection service)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var webViewHandler = new WebViewHandler();
            service.AddSingleton<IWebViewRequestHandler>(webViewHandler);
            var builder = new PolarShadowBuilder();
            var polarShadow = builder.AddSupported()
                .ConfigureItem<ISiteItemBuilder>(f =>
                {
                    f.WebViewHandler = webViewHandler;
                    f.RequestRules.Add(new RequestRule(Requests.Detail) { Writings = new List<IContentWriting> { new DetailContentWriting() } });
                    f.RequestRules.Add(new RequestRule(Requests.Detail) { NextRequst = Requests.Detail });
                    f.RequestRules.Add(new RequestRule(Requests.Search) { NextRequst = Requests.Detail });
                    f.RequestRules.Add(new RequestRule("category_*") { NextRequst = Requests.Detail });
                })
                .ConfigureItem<IParameterItemBuilder>(f =>
                {
                    var info =  GetResourceStream(new Uri("pack://application:,,,/Assets/PrefabParams.json"));
                    using var doc = JsonDocument.Parse(info.Stream);
                    f.PrefabParameters = new KeyValueParameter
                    {
                        doc.RootElement.Clone()
                    };
                }).Build();
            service.AddSingleton(polarShadow);
        }
    }

}
