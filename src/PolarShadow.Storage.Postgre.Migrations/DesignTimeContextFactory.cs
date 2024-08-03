﻿using Microsoft.EntityFrameworkCore;
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
            var builder = new DbContextOptionsBuilder<PolarShadowDbContext>().UseNpgsql("",op=> op.MigrationsAssembly(typeof(DesignTimeContextFactory).Assembly.FullName));
            return new PolarShadowDbContext(builder.Options);
        }
    }
}
