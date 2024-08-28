using Microsoft.EntityFrameworkCore;
using PolarShadow.Services;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class HistoryService : SyncAbleService<HistoryModel>, IDbHistoryService
    {
        private IDbContextFactory<PolarShadowDbContext> _contextFactory;
        public HistoryService(IDbContextFactory<PolarShadowDbContext> contextFactory):base(contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task AddOrUpdateAsync(HistoryModel model)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var data = await dbContext.Histories.AsNoTracking().FirstOrDefaultAsync(f=>f.ResourceName == model.ResourceName);
            if (data != null)
            {
                model.Id = data.Id;
            }
            model.UpdateTime = DateTime.Now;
            dbContext.Histories.Update(model);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Histories.Where(f => f.Id == id).ExecuteDeleteAsync();
        }

        public async Task<HistoryModel> GetByIdAsync(long id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Histories.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<HistoryModel> GetByResourceNameAsync(string reourceName)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Histories.AsNoTracking().FirstOrDefaultAsync(f=> f.ResourceName == reourceName);
        }

        public async Task<ICollection<HistoryModel>> GetListPageAsync(int page, int pageSize, string filter = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var skip = (page - 1) * pageSize;
            if (string.IsNullOrEmpty(filter))
            {
                return await context.Histories.AsNoTracking().OrderByDescending(f => f.UpdateTime).Skip(skip).Take(pageSize).ToListAsync();
            }
            else
            {
                return await context.Histories.Where(f => EF.Functions.Like(f.ProgressDesc, $"%{filter}%"))
                    .OrderByDescending(f=>f.UpdateTime)
                    .Skip(skip)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
    }
}
