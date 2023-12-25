using Microsoft.EntityFrameworkCore;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class HistoryService : IHistoryService
    {
        private IDbContextFactory<PolarShadowDbContext> _contextFactory;
        public HistoryService(IDbContextFactory<PolarShadowDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task AddOrUpdateAsync(HistoryModel model)
        {
            using var dbContext = _contextFactory.CreateDbContext();
            model.UpdateTime = DateTime.Now;
            dbContext.Histories.Update(model);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Histories.Where(f => f.Id == id).ExecuteDeleteAsync();
        }

        public async Task<HistoryModel> GetByIdAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Histories.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<HistoryModel> GetByResourceIdAsync(int resourceId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Histories.FirstOrDefaultAsync(f => f.ResourceId == resourceId);
        }

        public async Task<ICollection<HistoryModel>> GetListPageAsync(int page, int pageSize, string filter = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var skip = (page - 1) * pageSize;
            if (string.IsNullOrEmpty(filter))
            {
                return await context.Histories.OrderByDescending(f => f.UpdateTime).Skip(skip).Take(pageSize).ToListAsync();
            }
            else
            {
                return await context.Histories.Where(f => EF.Functions.Like(f.ProgressDesc, $"%{filter}%"))
                    .OrderByDescending(f=>f.UpdateTime)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();
            }
        }
    }
}
