using Microsoft.EntityFrameworkCore;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class SyncAbleService<T> : ISyncAble<T> where T : class
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        public SyncAbleService(IDbContextFactory<PolarShadowDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task UploadAsync(ICollection<T> data)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            await dbContext.Set<T>().ExecuteDeleteAsync();
            dbContext.AddRange(data);
            await dbContext.SaveChangesAsync();
        }
    }
}
