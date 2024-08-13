using Microsoft.EntityFrameworkCore;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal abstract class SyncAbleService<T> : ISyncAble<T> where T : class
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        public SyncAbleService(IDbContextFactory<PolarShadowDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<ICollection<T>> DownloadAsync()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            return await dbContext.Set<T>().ToListAsync();
        }

        public abstract Task UploadAsync(ICollection<T> data);
    }
}
