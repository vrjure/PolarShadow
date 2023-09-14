using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class PolarShadowDbContext : DbContext
    {
        public PolarShadowDbContext(DbContextOptions<PolarShadowDbContext> options) : base(options)
        { 
            
        }

        public DbSet<SiteModel> Sites { get; set; }
        public DbSet<RequestModel> Requests { get; set; }
        public DbSet<ResourceModel> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SiteModel>().HasKey(f => f.Name);
            modelBuilder.Entity<RequestModel>().HasKey(f => new { f.Name, f.SiteName});

            modelBuilder.Entity<ResourceModel>().HasKey(f => f.Id);
            modelBuilder.Entity<ResourceModel>().Property(e => e.SrcType).HasConversion<string>();

        }
    }
}
