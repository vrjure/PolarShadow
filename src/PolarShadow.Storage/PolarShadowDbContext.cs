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

        public DbSet<SiteEntity> Sites { get; set; }
        public DbSet<RequestEntity> Requests { get; set; }
        public DbSet<ResourceEntity> Resources { get; set; }
        public DbSet<EpisodeEntity> Episodes { get; set; }
        public DbSet<LinkEntity> Links { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ResourceEntity>()
                .Property(e => e.SrcType)
                .HasConversion<string>();

            modelBuilder.Entity<LinkEntity>()
                .Property(e => e.SrcType)
                .HasConversion<string>();

        }
    }
}
