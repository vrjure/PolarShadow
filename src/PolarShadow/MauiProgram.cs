using PolarShadow.Core;
using PolarShadow.Pages.ViewModels;
using PolarShadow.ResourcePack;
using CommunityToolkit.Maui;
using PolarShadow.Pages;
using NLog.Extensions.Logging;
using PolarShadow.Storage;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace PolarShadow;

public static class MauiProgram
{
    private static string _optionFileName = "source.json";
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

        builder.Services.AddSingleton(CreatePolarShadowBuilder());
        builder.Services.AddTransient(sp =>
        {
            var b = sp.GetRequiredService<IPolarShadowBuilder>();
            return b.Build();
        });

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
        services.AddTransientWithShellRoute<SourceManagerPage, SourceManagerViewModel>(nameof(SourceManagerPage));
    }

    private static IPolarShadowBuilder CreatePolarShadowBuilder()
    {
        var sourcePath = Path.Combine(FileSystem.AppDataDirectory, _optionFileName);
        using var fs = new FileStream(sourcePath, FileMode.OpenOrCreate, FileAccess.Read);
        if (fs.Length == 0)
        {
            var builder = new PolarShadowBuilder();
            return builder;
        }
        else
        {
            var option = JsonSerializer.Deserialize<PolarShadowOption>(fs, JsonOption.DefaultSerializer);
            var builder = new PolarShadowBuilder(option);
            return builder;
        }

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

    public static void SavePolarShadowOption(PolarShadowOption option)
    {
        var sourcePath = Path.Combine(FileSystem.AppDataDirectory, _optionFileName);
        if (!File.Exists(sourcePath))
        {
            File.Create(sourcePath);
        }
        using var fs = new FileStream(sourcePath, FileMode.Truncate, FileAccess.Write);
        var s = JsonSerializer.Serialize(option, JsonOption.DefaultSerializer);
        using var sw = new StreamWriter(fs);
        sw.Write(s);
        sw.Flush();
    }
}
