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
            using var dbContext = _dbFactory.CreateDbContext();
            return await dbContext.Sources.OrderByDescending(f=>f.UpdateTime).FirstOrDefaultAsync();
        }

        public async Task SaveSourceAsync(SourceModel source)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            dbContext.Sources.Update(source);
            await dbContext.SaveChangesAsync();
        }

        public override async Task UploadAsync(ICollection<SourceModel> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }
            using var dbContext = _dbFactory.CreateDbContext();
            var local = await dbContext.Sources.OrderByDescending(f => f.UpdateTime).AsNoTracking().FirstOrDefaultAsync();
            if (local == null)
            {
                dbContext.Sources.AddRange(data);
            }
            else
            {
                foreach (var item in data)
                {
                    if (item.UpdateTime.ToUniversalTime() > local.UpdateTime.ToUniversalTime())
                    {
                        dbContext.Sources.Update(item);
                    }
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
