using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using PolarShadow.Api;
using PolarShadow.Api.Swagger;
using PolarShadow.Core;
using PolarShadow.Storage;
using PolarShadow.Storage.Postgre.Migrations;
using PolarShadow.Services;
using PolarShadow.Api.Utilities;
using PolarShadow.Api.Utilities.Extensions;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings(reloadOnChange:true).GetCurrentClassLogger();
logger.Info("Server Starting");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<HttpResponseFilter>();
        options.OutputFormatters.RemoveType<StringOutputFormatter>();
        options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
    })
    .AddJsonOptions(option =>
    {
        JsonOption.Default(option.JsonSerializerOptions);
    });
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.OperationFilter<SwaggerGenOperationFilter>();
    });

    var settingSection = builder.Configuration.GetSection("PolarShadowSetting");
    var setting = settingSection.Get<PolarShadowSetting>();
    builder.Services.Configure<PolarShadowSetting>(settingSection);
    

    builder.Services.AddDbContextFactory<PolarShadowDbContext>(builder =>
    {
        builder.UseNpgsql(setting!.ConnectionString, opBuilder => opBuilder.MigrationsAssembly(typeof(DesignTimeContextFactory).Assembly.FullName));
    });

    builder.Services.RegisterStorageService();
    builder.Services.AddSingleton<FileSafeOperate>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.MapControllers();

    var dbFactory = app.Services.GetRequiredService<IDbContextFactory<PolarShadowDbContext>>();
    using (var dbContext = dbFactory.CreateDbContext())
    {
        dbContext.Database.Migrate();
    }

    app.InitializeFileSafeOperate();

    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex, "Server stopped because of a exception");
	throw;
}
finally
{
    LogManager.Shutdown();
}
