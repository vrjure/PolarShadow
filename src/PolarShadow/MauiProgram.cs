using PolarShadow.Core;
using PolarShadow.Pages.ViewModels;
using PolarShadow.ResourcePack;
using CommunityToolkit.Maui;
using PolarShadow.Pages;
using NLog.Extensions.Logging;
using PolarShadow.Storage;
using Microsoft.EntityFrameworkCore;

namespace PolarShadow;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureServices();
        return builder.Build().InitializeApp();
	}

    private static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
    {
        ConfigureLogger(builder.Services);

        builder.Services.AddSingleton(BuildIPolarShadow());

        RegisterViewModels(builder.Services);

        builder.Services.AddDbContextFactory<PolarShadowDbContext>(options =>
        {
            var path = Path.Combine(FileSystem.Current.AppDataDirectory, "polar.db3");
            options.UseSqlite($"Data Source={path}", op => op.MigrationsAssembly(typeof(PolarShadowDbContext).Assembly.FullName));
        });

        builder.Services.AddStorageServices();

        return builder;
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransientWithShellRoute<SearchPage, SearchPageViewModel>(nameof(SearchPage));
        services.AddTransientWithShellRoute<VideoDetailPage, VideoDetailViewModel>(nameof(VideoDetailPage));
        services.AddTransientWithShellRoute<WebBrowserPage, WebBrowserPageViewModel>(nameof(WebBrowserPage));
        services.AddTransientWithShellRoute<ConfigurePage, ConfigPageViewModel>(nameof(ConfigurePage));
        services.AddTransientWithShellRoute<MyCollectionPage, MyCollectionViewModel>(nameof(MyCollectionPage));
    }

    private static IPolarShadow BuildIPolarShadow()
    {
        var builder = new PolarShadowBuilder();
        builder.AutoSite(typeof(NewZMZSite).Assembly);
        return builder.Build();
    }

    private static void ConfigureLogger(IServiceCollection services)
    {
        services.AddLogging(loggerBuilder =>
        {
            loggerBuilder.AddNLog();
        });
    }

    private static MauiApp InitializeApp(this MauiApp app)
    {
        var dbFactory = app.Services.GetRequiredService<IDbContextFactory<PolarShadowDbContext>>();
        using (var context = dbFactory.CreateDbContext())
        {
            context.Database.Migrate();
        }
        return app;
    }
}
