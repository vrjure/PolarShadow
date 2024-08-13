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
            using var dbContext = _contextFactory.CreateDbContext();
            var data = await dbContext.Histories.FirstOrDefaultAsync(f=>f.ResourceName == model.ResourceName);
            if (data != null)
            {
                model.Id = data.Id;
            }
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

        public async Task<HistoryModel> GetByResourceNameAsync(string reourceName)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Histories.FirstOrDefaultAsync(f=> f.ResourceName == reourceName);
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

        public override async Task UploadAsync(ICollection<HistoryModel> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }

            using var dbContext = _contextFactory.CreateDbContext();
            var dbData = await dbContext.Histories.ToListAsync();
            if (!dbData.Any())
            {
                foreach (var item in data)
                {
                    item.Id = 0;
                }

                dbContext.Histories.AddRange(data);
            }
            else
            {
                var addList = data.ExceptBy(dbData.Select(f => f.ResourceName), f => f.ResourceName);
                var union = dbData.UnionBy(data, f => f.ResourceName);

                var updateList = new List<HistoryModel>();
                foreach (var item in union)
                {
                    var db = dbData.First(f => f.ResourceName == item.ResourceName);
                    var remote = data.First(f => f.ResourceName == item.ResourceName);
                    if (remote.UpdateTime.ToUniversalTime() > db.UpdateTime.ToUniversalTime())
                    {
                        db.UpdateTime = remote.UpdateTime;
                        updateList.Add(db);
                    }
                }

                dbContext.Histories.UpdateRange(updateList);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
