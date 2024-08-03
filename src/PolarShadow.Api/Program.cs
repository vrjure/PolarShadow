using Microsoft.EntityFrameworkCore;
using PolarShadow.Api;
using PolarShadow.Core;
using PolarShadow.Storage;
using PolarShadow.Storage.Postgre.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(option =>
    {
        JsonOption.Default(option.JsonSerializerOptions);
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var setting = builder.Configuration.GetSection("PolarShadowSetting").Get<PolarShadowSetting>();

builder.Services.AddDbContextFactory<PolarShadowDbContext>(builder =>
{
    builder.UseNpgsql(setting!.ConnectionString, opBuilder => opBuilder.MigrationsAssembly(typeof(DesignTimeContextFactory).Assembly.FullName));
});

builder.Services.RegisterStorageService();

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

app.Run();
