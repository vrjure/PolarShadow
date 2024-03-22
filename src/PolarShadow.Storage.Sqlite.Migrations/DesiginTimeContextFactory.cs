using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage.Sqlite.Migrations
{
    internal class DesiginTimeContextFactory : IDesignTimeDbContextFactory<PolarShadowDbContext>
    {
        public PolarShadowDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PolarShadowDbContext>().UseSqlite("Data Source=./polar.db3");
            return new PolarShadowDbContext(builder.Options);
        }
    }
}
