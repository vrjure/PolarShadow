using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal abstract class SyncAbleService<T> : ISyncAble<T> where T : class, ISyncAbleModel, IKey
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        public SyncAbleService(IDbContextFactory<PolarShadowDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<ICollection<T>> DownloadAsync(DateTime updateTime)
        {
            var utc = updateTime.ToUniversalTime();
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.Set<T>().Where(f=>f.UpdateTime > updateTime).ToListAsync();
        }

        public async Task<DateTime> GetLastTimeAsync()
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            var last = await dbContext.Set<T>().OrderBy(f => f.UpdateTime).LastOrDefaultAsync();
            if (last == null)
            {
                return DateTime.MinValue;
            }

            return last.UpdateTime;
        }

        public async Task UploadAsync(ICollection<T> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }

            using var dbContext = await _dbFactory.CreateDbContextAsync();
            using var trans = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var dataTime = data.OrderBy(f => f.UpdateTime).FirstOrDefault().UpdateTime.ToUniversalTime();
                var dbList = await dbContext.Set<T>().AsNoTracking().Where(f => f.UpdateTime > dataTime).ToListAsync();
                var addList = data.ExceptBy(dbList.Select(f => f.Id), f => f.Id);
                var updateList = data.IntersectBy(dbList.Select(f=>f.Id), f => f.Id);

                if (addList.Any())
                {
                    dbContext.Set<T>().AddRange(addList);
                }

                if (updateList.Any())
                {
                    dbContext.Set<T>().UpdateRange(updateList);
                }

                await dbContext.SaveChangesAsync();

                await trans.CommitAsync();
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw;
            }
        }
    }
}
