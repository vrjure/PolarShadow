﻿using Avalonia;
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
using PolarShadow.Essentials;
using Avalonia.Controls.Notifications;
using PolarShadow.Storage;
using Microsoft.EntityFrameworkCore;
using System.IO;
using PolarShadow.Resources;
using System.Text;
using PolarShadow.Handlers;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using Avalonia.Platform;
using System.Text.Json;
using PolarShadow.StoragePicker;
using PolarShadow.Dispatcher;
using PolarShadow.Notifaction;
using PolarShadow.Storage.Sqlite.Migrations;

namespace PolarShadow;

public partial class App : Application
{
    public IServiceCollection ServiceCollection = new ServiceCollection();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void RegisterServices()
    {
        base.RegisterServices();
        ConfigureService();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var service = ServiceCollection.BuildServiceProvider();

        Ioc.Default.ConfigureServices(service);

        var topLevelService = Ioc.Default.GetRequiredService<ITopLevelService>();
        using var db = Ioc.Default.GetRequiredService<IDbContextFactory<PolarShadowDbContext>>().CreateDbContext();
        db.Database.Migrate();
        var nav = Ioc.Default.GetRequiredService<INavigationService>();

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Ioc.Default.GetRequiredService<MainWindow>();
            topLevelService.SetTopLevel(desktop.MainWindow);
            desktop.MainWindow.Deactivated += MainWindow_Deactivated;
            nav.Navigate<TopLayoutViewModel>(MainWindowViewModel.NavigationName);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = Ioc.Default.GetRequiredService<TopLayoutView>();
            topLevelService.SetTopLevelFactory(() => singleViewPlatform.MainView);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void MainWindow_Deactivated(object sender, EventArgs e)
    {
        FileCacheFlush();
    }

    private void ConfigureService()
    {
        var service = ServiceCollection;

        RegisterView(service);

        RegisterPolarShadow(service);

        RegisterConfigureDatabase(service);

        RegisterUtilities(service);

        RegisterNativeControls(service);
    }

    private void RegisterUtilities(IServiceCollection service)
    {
        service.RegisterVLC();
        service.RegisterCache(new FileCacheOptions { CacheFolder = PolarShadowApp.CacheFolder });
        service.AddSingleton<INavigationService, NavigationService>();
        service.AddSingleton<IStorageItemPicker, StorageItemPicker>();
        service.AddSingleton<ITopLevelService, TopLevelService>();
        service.AddSingleton<INotificationManager, NotificationManager>();
        service.AddSingleton<IDispatcherUI, DispatcherUI>();
        service.AddSingleton<IMessageService, NotificationService>();
        service.AddPolarShadowService();

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
                f.RequestRules.Add(new RequestRule(Requests.Detail) { Writings = new List<IContentWriting>{ new DetailContentWriting() }});
                f.RequestRules.Add(new RequestRule(Requests.Detail) { NextRequst = Requests.Detail });
                f.RequestRules.Add(new RequestRule(Requests.Search) { NextRequst = Requests.Detail });
                f.RequestRules.Add(new RequestRule("category_*") { NextRequst = Requests.Detail });
            })
            .ConfigureItem<IParameterItemBuilder>(f =>
            {
                using var stream = AssetLoader.Open(new Uri("avares://PolarShadow/Assets/PrefabParams.json"));
                using var doc = JsonDocument.Parse(stream);
                f.PrefabParameters = new KeyValueParameter
                {
                    doc.RootElement.Clone()
                };
            }).Build();
        service.AddSingleton(polarShadow);
    }

    private void RegisterConfigureDatabase(IServiceCollection service)
    {
        service.AddDbContextFactory<PolarShadowDbContext>(op =>
        {
            op.UseSqlite($"Data Source={PolarShadowApp.DbFile}", op => op.MigrationsAssembly(typeof(DesignTimeContextFactory).Assembly.FullName));
        });
    }

    private void RegisterNativeControls(IServiceCollection service)
    {
        service.AddLibVLC();
        service.AddWebViewOptions(new WebViewOptions
        {
            UserDataFolder = Path.Combine(PolarShadowApp.AppDataFolder, "webview")
        });
    }

    public static void FileCacheFlush()
    {
        var fileCache = Ioc.Default.GetService<IFileCache>();
        fileCache?.Flush();
    }
}
