using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PolarShadow.Core;
using PolarShadow.Storage;
using System.Text.Json;

namespace PolarShadow
{
    public static class MauiProgram
    {
        private static string _optionFileName = "source.json";
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
            builder.Services.AddSingleton<IStateContext, StateContext>();

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
            var sourcePath = Path.Combine(FileSystem.AppDataDirectory, _optionFileName);
            using var fs = new FileStream(sourcePath, FileMode.OpenOrCreate, FileAccess.Read);
            if (fs.Length == 0)
            {
                var builder = new PolarShadowBuilder();
                return builder.AddDefaultAbilities();
            }
            else
            {
                var option = JsonSerializer.Deserialize<PolarShadowOption>(fs, JsonOption.DefaultSerializer);
                var builder = new PolarShadowBuilder(option);
                return builder.AddDefaultAbilities();
            }

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