using PolarShadow.Core;
using PolarShadow.Pages.ViewModels;
using PolarShadow.ResourcePack;
using CommunityToolkit.Maui;
using PolarShadow.Pages;
using NLog;
using NLog.Extensions.Logging;

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
            });

        ConfigureServices(builder);
        return builder.Build();
	}

    private static void ConfigureServices(MauiAppBuilder builder)
    {
        ConfigureLogger(builder.Services);

        builder.Services.AddSingleton(BuildIPolarShadow());

        RegisterViewModels(builder.Services);
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransientWithShellRoute<SearchPage, SearchPageViewModel>(nameof(SearchPage));
        services.AddTransientWithShellRoute<VideoDetailPage, VideoDetailViewModel>(nameof(VideoDetailPage));
        services.AddTransientWithShellRoute<WebBrowserPage, WebBrowserPageViewModel>(nameof(WebBrowserPage));
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
}
