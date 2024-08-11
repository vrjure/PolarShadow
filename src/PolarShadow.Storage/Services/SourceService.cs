using Microsoft.EntityFrameworkCore;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class SourceService : IDbSourceService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        public SourceService(IDbContextFactory<PolarShadowDbContext> dbFactroy)
        {
            _dbFactory = dbFactroy;
        }

        public async Task<SourceModel> GetSouuceAsync()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            return await dbContext.Sources.OrderByDescending(f=>f.UpdateTime).FirstOrDefaultAsync();
        }

        public async Task SaveSourceAsync(SourceModel source)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            dbContext.Sources.Update(source);
            await dbContext.SaveChangesAsync();
        }

        public async Task UploadAsync(ICollection<SourceModel> data)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            await dbContext.Sources.ExecuteDeleteAsync();
            dbContext.Sources.AddRange(data);
            await dbContext.SaveChangesAsync();
        }
    }
}
