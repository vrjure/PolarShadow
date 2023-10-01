using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Core;
using PolarShadow.ViewModels;
using PolarShadow.Views;
using System;
using PolarShadow.Navigations;
using PolarShadow.Services;
using Avalonia.Controls.Notifications;
using PolarShadow.Storage;
using Microsoft.EntityFrameworkCore;
using System.IO;
using PolarShadow.Cache;
using PolarShadow.Resources;
using System.Text;
using AvaloniaWebView;
using PolarShadow.Handlers;
using Avalonia.NativeControls;

namespace PolarShadow;

public partial class App : Application
{
    public static string AppDataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PolarShadow");
    public static string ConfigFile => Path.Combine(AppDataFolder, "config.json");
    public static string DbFile => Path.Combine(AppDataFolder, "polar.db");
    public static string CacheFolder => Path.Combine(AppDataFolder, "cache");
    public static string PreferenceFolder => Path.Combine(AppDataFolder, "preference");

    public static IServiceProvider Service => (App.Current as App)._services;

    private IServiceProvider _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        AvaloniaWebViewBuilder.Initialize(default);
        NativeControlHandlers.Initialize();
    }

    public override void RegisterServices()
    {
        base.RegisterServices();
        ConfigureService();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var topLevelService = _services.GetRequiredService<ITopLevelService>();
        var nav = _services.GetRequiredService<INavigationService>();

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _services.GetRequiredService<MainWindow>();
            topLevelService.SetTopLevel(desktop.MainWindow);
            nav.Navigate<TopLayoutViewModel>(MainWindowViewModel.NavigationName);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = _services.GetRequiredService<TopLayoutView>();
            topLevelService.SetTopLevelFactory(() => singleViewPlatform.MainView);
            nav.Navigate<MainViewModel>(TopLayoutViewModel.NavigationName);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureService()
    {
        var service = new ServiceCollection();

        RegisterView(service);

        RegisterPolarShadow(service);

        RegisterConfigureDatabase(service);

        RegisterUtilities(service);

        RegisterCache(service);

        RegisterVLC(service);

        _services = service.BuildServiceProvider();

    }

    private void RegisterUtilities(IServiceCollection service)
    {
        service.AddSingleton<INavigationService, NavigationService>();
        service.AddSingleton<IStorageService, StorageService>();
        service.AddSingleton<ITopLevelService, TopLevelService>();
        service.AddSingleton<INotificationManager, NotificationManager>();
        service.AddSingleton<IPreference, DbPreference>();
    }

    private void RegisterView(IServiceCollection service)
    {
        service.RegisterSingletonViewWithModel<TopLayoutView, TopLayoutViewModel>();
        service.RegisterSingletonViewWithModel<MainWindow, MainWindowViewModel>();
        service.RegisterTransientViewWithModel<MainView, MainViewModel>();
        service.RegisterTransientViewWithModel<BookshelfView, BookshelfViewModel>();
        service.RegisterTransientViewWithModel<BookSourceView, BookSourceViewModel>();
        service.RegisterTransientViewWithModel<BookSourceDetailView, BookSourceDetailViewModel>();
        service.RegisterTransientViewWithModel<SearchView, SearchViewModel>();
        service.RegisterTransientViewWithModel<DetailView, DetailViewModel>();
        service.RegisterTransientViewWithModel<DiscoverView, DiscoverViewModel>();
        service.RegisterTransientViewWithModel<DiscoverDetailView, DiscoverDetailViewModel>();
        service.RegisterTransientViewWithModel<MineView, MineViewModel>();
        service.RegisterTransientViewWithModel<VideoPlayerView, VideoPlayerViewModel>();
    }

    private void RegisterPolarShadow(IServiceCollection service)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var webViewHandler = new WebViewHandler();
        service.AddSingleton<IWebViewRequestHandler>(webViewHandler);
        var builder = new PolarShadowBuilder();
        var polarShadow = builder.ConfigureAllSupported()
            .ConfigureSiteItem(f =>
            {
                f.Writings.Add(new DetailContentWriting());
                f.WebViewHandler = webViewHandler;
            }).Build();
        service.AddSingleton(polarShadow);
    }

    private void RegisterConfigureDatabase(IServiceCollection service)
    {
        service.AddDbContextFactory<PolarShadowDbContext>(op =>
        {
            op.UseSqlite($"Data Source={DbFile}", op => op.MigrationsAssembly(typeof(App).Assembly.FullName));
        });
        service.RegisterStorageService();
    }

    private void RegisterCache(IServiceCollection service)
    {
        service.AddMemoryCache();
        service.AddSingleton<IFileCache>(new FileCache(CacheFolder));
        service.AddSingleton<IBufferCache, BufferCache>();

        if (!Directory.Exists(CacheFolder))
        {
            Directory.CreateDirectory(CacheFolder);
        }
    }

    private void RegisterVLC(IServiceCollection service)
    {
        service.AddLibVLC();
    }
}
