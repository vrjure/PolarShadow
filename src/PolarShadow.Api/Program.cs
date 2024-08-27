using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using PolarShadow.Api;
using PolarShadow.Api.Swagger;
using PolarShadow.Storage;
using PolarShadow.Storage.Postgre.Migrations;
using PolarShadow.Services;
using PolarShadow.Api.Utilities;
using PolarShadow.Api.Utilities.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.OpenApi.Models;

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
        JsonOptions.Default(option.JsonSerializerOptions);
    });
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.OperationFilter<SwaggerGenOperationFilter>();
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "JWT Authentication",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT"
        });
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[]{}
            }
        });
    });

    var settingSection = builder.Configuration.GetSection("PolarShadowSetting");
    var setting = settingSection.Get<PolarShadowSetting>();
    builder.Services.Configure<PolarShadowSetting>(settingSection);
    

    builder.Services.AddDbContextFactory<PolarShadowDbContext>(builder =>
    {
        builder.UseNpgsql(setting!.ConnectionString, opBuilder => opBuilder.MigrationsAssembly(typeof(DesignTimeContextFactory).Assembly.FullName));
    });

    var jwtSection = builder.Configuration.GetSection("jwt");
    var jwtOptions = jwtSection.Get<JWTOptions>() ?? new JWTOptions();
    builder.Services.Configure<JWTOptions>(jwtSection);

    builder.Services.AddAuthentication(op =>
    {
        op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(op =>
    {
        op.RequireHttpsMetadata = false;
        op.SaveToken = false;
        op.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
        
    });

    builder.Services.AddAuthorization(op =>
    {
        op.AddPolicy(Policies.Client, policy =>
        {
            policy.RequireClaim(JWTClaimTypes.ClientId, jwtOptions.ClientId);
            policy.RequireUserName(jwtOptions.UserName);
        });
    });

    builder.Services.AddPolarShadowService();
    builder.Services.AddUntities();

    var app = builder.Build();

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
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
