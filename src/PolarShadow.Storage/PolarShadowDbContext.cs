using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
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
        public DbSet<SourceVersionModel> SourceVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ResourceModel>().HasKey(f => f.Id);
            modelBuilder.Entity<ResourceModel>().Property(e => e.SrcType).HasConversion<string>();
            modelBuilder.Entity<ResourceModel>().Property(e => e.ImageSrcHeaders).HasConversion(
                f => JsonSerializer.Serialize(f, JsonOption.DefaultSerializer),
                f => JsonSerializer.Deserialize<IDictionary<string, string>>(f, JsonOption.DefaultSerializer));

            modelBuilder.Entity<PreferenceModel>().HasKey(f => f.Key);

            modelBuilder.Entity<HistoryModel>().HasKey(f => f.Id);
            modelBuilder.Entity<HistoryModel>().HasIndex(f => f.ResourceName);

            modelBuilder.Entity<SourceVersionModel>().HasKey(f => f.Id);
        }
    }
}
