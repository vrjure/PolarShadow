using Microsoft.EntityFrameworkCore;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class PolarShadowDbContext : DbContext
    {
        public PolarShadowDbContext(DbContextOptions<PolarShadowDbContext> options) : base(options)
        { 
            
        }

        public DbSet<ResourceModel> Resources { get; set; }
        public DbSet<PreferenceModel> Preferences { get; set; }
        public DbSet<HistoryModel> Histories { get; set; }
        public DbSet<SourceModel> Sources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (IsPostgreProvider(this))
            {

            }
            modelBuilder.Entity<ResourceModel>(builder =>
            {
                builder.HasKey(f => f.Id);
                builder.Property(f => f.SrcType).HasConversion<string>();
                builder.Property(f => f.ImageSrcHeaders)
                .HasConversion(
                    f => JsonSerializer.Serialize(f, JsonOptions.DefaultSerializer),
                    f => JsonSerializer.Deserialize<IDictionary<string, string>>(f, JsonOptions.DefaultSerializer));
            });

            modelBuilder.Entity<PreferenceModel>().HasKey(f => f.Key);

            modelBuilder.Entity<HistoryModel>(builder =>
            {
                builder.HasKey(f => f.Id);
                builder.HasIndex(f => f.ResourceName);
                if (ISSQliteProvider(this))
                {
                    builder.Property(f => f.UpdateTime).HasConversion(
                            f => DateTimeToString(f),
                            f => StringToDateTime(f));
                }
                else if (IsPostgreProvider(this))
                {
                    builder.Property(f => f.UpdateTime).HasConversion(
                        f => f.ToUniversalTime(),
                        f => f);
                }
            });

            modelBuilder.Entity<SourceModel>(builder =>
            {
                builder.HasKey(f => f.Id);
                if (ISSQliteProvider(this))
                {
                    builder.Property(f => f.UpdateTime).HasConversion(
                            f => DateTimeToString(f),
                            f => StringToDateTime(f));
                    builder.Property(f => f.CreateTime).HasConversion(
                            f => DateTimeToString(f),
                            f => StringToDateTime(f));
                } 
                else if (IsPostgreProvider(this))
                {
                    builder.Property(f => f.UpdateTime).HasConversion(
                            f => f.ToUniversalTime(),
                            f => f);
                    builder.Property(f => f.CreateTime).HasConversion(
                            f => f.ToUniversalTime(),
                            f => f);
                }
            });
        }

        private static string DateTimeToString(DateTime d)
        {
            return d.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        private static DateTime StringToDateTime(string datetime)
        {
            return DateTime.Parse(datetime);
        }

        private static bool ISSQliteProvider(DbContext contenxt)
        {
            return contenxt.Database.ProviderName.Equals("Microsoft.EntityFrameworkCore.Sqlite", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPostgreProvider(DbContext context)
        {
            return context.Database.ProviderName.Equals("Npgsql.EntityFrameworkCore.PostgreSQL", StringComparison.OrdinalIgnoreCase);
        }
    }
}
