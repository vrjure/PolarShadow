using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.ViewModels;
using PolarShadow.Views;
using System;

namespace PolarShadow;

public partial class App : Application
{
    public static IServiceProvider Service => (App.Current as App)._services;

    private IServiceProvider _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        BuildService();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _services.GetRequiredService<MainWindow>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = _services.GetRequiredService<MainView>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void BuildService()
    {
        var service = new ServiceCollection();

        service.AddNavigation();
        service.RegisterSingletonView<MainWindow>();
        service.RegisterTransientViewWithModel<MainView, MainViewModel>();
        service.RegisterTransientViewWithModel<BookshelfView, BookshelfViewModel>();


        _services = service.BuildServiceProvider();
    }
}
