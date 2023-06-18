using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Videos;
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

        public DbSet<VideoDetailEntity> MyCollection { get; set; }
        public DbSet<WatchRecord> Record { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VideoDetailEntity>().HasKey(f => f.Name);

            modelBuilder.Entity<WatchRecord>().HasKey(f => f.Id);
            modelBuilder.Entity<WatchRecord>().HasIndex(f => f.Name);
        }
    }
}
