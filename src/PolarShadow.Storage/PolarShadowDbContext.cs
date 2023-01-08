using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
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

        public DbSet<VideoSummary> MyCollection { get; set; }
        public DbSet<WatchRecord> Record { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VideoSummary>().HasKey(f => f.Name);

            modelBuilder.Entity<WatchRecord>().HasKey(f => f.Id);
            modelBuilder.Entity<WatchRecord>().HasIndex(f => f.Name);
        }
    }
}
