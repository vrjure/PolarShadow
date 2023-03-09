using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class MyCollectionService : IMyCollectionService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        public MyCollectionService(IDbContextFactory<PolarShadowDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public async Task AddToMyCollectionAsync(VideoDetail summary)
        {
            using (var context = _dbFactory.CreateDbContext())
            {
                if (await context.MyCollection.AnyAsync(f=>f.Name == summary.Name))
                {
                    return;
                }
                context.MyCollection.Add(summary);
                await context.SaveChangesAsync();
            }
        }

        public async Task<ICollection<VideoSummary>> GetMyCollectionAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            using (var context = _dbFactory.CreateDbContext())
            {
                return await context.MyCollection.Skip(skip).Take(pageSize).AsNoTracking().ToListAsync();
            }
        }

        public async Task<bool> HasAsync(VideoSummary summary)
        {
            using (var context = _dbFactory.CreateDbContext())
            {
                return await context.MyCollection.Where(f=>f.Name== summary.Name).AnyAsync();
            }
        }

        public async Task RemoveFromMyCollectionAsync(string name)
        {
            using (var context = _dbFactory.CreateDbContext())
            {
                context.MyCollection.Remove(new VideoSummary { Name = name});
                await context.SaveChangesAsync();
            }
        }
    }
}
