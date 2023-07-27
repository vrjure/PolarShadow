using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PolarShadow.Core;
using PolarShadow;
using PolarShadow.Storage;
using System.Text.Json;
using PolarShadow.Core.Aria2;
using PolarShadow.Cache;
using Microsoft.Maui.Hosting;
using PolarShadow.Videos;
using System.Text;

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
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddAntDesign();
            builder.Services.AddHttpClient("default", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62");
            });
            builder.Services.AddScoped<IStateContext, StateContext>();
            builder.Services.AddScoped<INavigationService, NavigationService>();
            builder.Services.AddScoped<IHttpFileResource, HttpFileResource>();
            builder.Services.AddScoped<IHttpResource, HttpResource>();

            AddPolarShadow(builder.Services);

            builder.Services.AddDbContextFactory<PolarShadowDbContext>(options =>
            {
                var path = Path.Combine(FileSystem.Current.AppDataDirectory, "polar.db3");
                options.UseSqlite($"Data Source={path}", op => op.MigrationsAssembly(typeof(PolarShadowDbContext).Assembly.FullName));
            });

            builder.Services.AddStorageServices();

            return builder;
        }

        private static void AddPolarShadow(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var builder = new PolarShadowBuilder();
            var webViewHandler = new WebViewHandler();
            services.AddSingleton<IWebViewRequestHandler>(webViewHandler);
            builder.ConfigureDefault().ConfigureSiteItem(itemBuilder =>
            {
                itemBuilder.WebViewHandler = webViewHandler;
            }).ConfigreVideo();

            var polarShadow = builder.Build();
            services.AddSingleton(polarShadow);
        }

        private static MauiApp InitializeApp(this MauiApp app)
        {
            var dbFactory = app.Services.GetRequiredService<IDbContextFactory<PolarShadowDbContext>>();
            using (var context = dbFactory.CreateDbContext())
            {
                context.Database.Migrate();
            }

            var polarShadow = app.Services.GetRequiredService<IPolarShadow>();
            polarShadow.ReadFromFile();
            return app;
        }
    }
}