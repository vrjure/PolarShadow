using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PolarShadow.Core;
using PolarShadow;
using PolarShadow.Storage;
using System.Text.Json;
using PolarShadow.Core.Aria2;
using PolarShadow.Cache;

namespace PolarShadow
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                }).ConfigureServices();          

            return builder.Build().InitializeApp();
        }

        private static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
        {
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddAntDesign();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IStateContext, StateContext>();
            builder.Services.AddScoped<INavigationService, NavigationService>();
            builder.Services.AddScoped<IImageCache, FileImageCache>();

            builder.Services.AddSingleton(CreatePolarShadowBuilder());
            builder.Services.AddTransient(sp =>
            {
                var b = sp.GetRequiredService<IPolarShadowBuilder>();
                return b.Build();
            });

            builder.Services.AddDbContextFactory<PolarShadowDbContext>(options =>
            {
                var path = Path.Combine(FileSystem.Current.AppDataDirectory, "polar.db3");
                options.UseSqlite($"Data Source={path}", op => op.MigrationsAssembly(typeof(PolarShadowDbContext).Assembly.FullName));
            });

            builder.Services.AddStorageServices();

            return builder;
        }

        private static IPolarShadowBuilder CreatePolarShadowBuilder()
        {
            var builder = new PolarShadowBuilder();
            return builder.ReadFromFile().AddDefaultAbilities();

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
}