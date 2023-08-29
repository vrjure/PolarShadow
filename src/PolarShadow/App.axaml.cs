using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Core;
using PolarShadow.ViewModels;
using PolarShadow.Views;
using PolarShadow.Videos;
using System;
using PolarShadow.Navigations;
using PolarShadow.Services;
using Avalonia.Controls.Notifications;

namespace PolarShadow;

public partial class App : Application
{
    public static IServiceProvider Service => (App.Current as App)._services;

    private IServiceProvider _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(_services, this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ConfigureService();

        var topLevelService = _services.GetRequiredService<ITopLevelService>();
        var nav = _services.GetRequiredService<INavigationService>();

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _services.GetRequiredService<MainWindow>();
            topLevelService.SetTopLevel(desktop.MainWindow);
            nav.Navigate<TopLayoutView>(MainWindowViewModel.NavigationName);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = _services.GetRequiredService<TopLayoutView>();
            topLevelService.SetTopLevelFactory(() => singleViewPlatform.MainView);
            nav.Navigate<MainView>(TopLayoutViewModel.NavigationName);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureService()
    {
        var service = new ServiceCollection();

        RegisterView(service);

        RegisterPolarShadow(service);

        RegisterUtilities(service);

        _services = service.BuildServiceProvider();

    }

    private void RegisterUtilities(IServiceCollection service)
    {
        service.AddSingleton<INavigationService, NavigationService>();
        service.AddSingleton<IStorageService, StorageService>();
        service.AddSingleton<ITopLevelService, TopLevelService>();
        service.AddSingleton<INotificationManager, NotificationManager>();
    }

    private void RegisterView(IServiceCollection service)
    {
        service.RegisterSingletonViewWithModel<MainWindow, MainWindowViewModel>();
        service.RegisterSingletonViewWithModel<TopLayoutView, TopLayoutViewModel>();

        service.RegisterTransientViewWithModel<MainView, MainViewModel>();
        service.RegisterTransientViewWithModel<BookshelfView, BookshelfViewModel>();
        service.RegisterTransientViewWithModel<BookSourceView, BookSourceViewModel>();

    }

    private void RegisterPolarShadow(IServiceCollection service)
    {
        var builder = new PolarShadowBuilder();
        builder.ConfigureDefault().ConfigreVideo();
        service.AddSingleton(builder.Build());
    }
}
