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
    internal class SourceService : SyncAbleService<SourceModel>, IDbSourceService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        public SourceService(IDbContextFactory<PolarShadowDbContext> dbFactroy) : base(dbFactroy)
        {
            _dbFactory = dbFactroy;
        }

        public async Task<SourceModel> GetSouuceAsync()
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.Sources.OrderByDescending(f=>f.UpdateTime).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task SaveSourceAsync(SourceModel source)
        {
            source.UpdateTime = DateTime.Now;
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            if (dbContext.Sources.Any(f=>f.Id == source.Id))
            {
                dbContext.Sources.Update(source);
            }
            else
            {
                dbContext.Sources.Add(source);
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
