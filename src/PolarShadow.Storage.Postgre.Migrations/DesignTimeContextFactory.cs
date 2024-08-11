using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage.Postgre.Migrations
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<PolarShadowDbContext>
    {
        public PolarShadowDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PolarShadowDbContext>().UseNpgsql("Host=localhost;Port=5432;Username=root;Password=123456;Database=PolarShadow;", op=> op.MigrationsAssembly(typeof(DesignTimeContextFactory).Assembly.FullName));
            return new PolarShadowDbContext(builder.Options);
        }
    }
}
