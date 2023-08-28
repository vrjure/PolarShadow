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
using PolarShadow.Storage;
using PolarShadow.Navigations;

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

        var storageService = _services.GetRequiredService<IStorageService>() as StorageService;

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _services.GetRequiredService<MainWindow>();
            storageService.TopLevel = desktop.MainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = _services.GetRequiredService<MainView>();
            storageService.VisualFactory = () => singleViewPlatform.MainView;
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
    }

    private void RegisterView(IServiceCollection service)
    {
        service.RegisterSingletonView<MainWindow>();
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
