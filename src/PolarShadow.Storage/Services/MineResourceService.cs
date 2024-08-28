using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PolarShadow.Resources;
using PolarShadow.Services;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class MineResourceService : SyncAbleService<ResourceModel>, IDbMineResourceService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbContextFactory;

        public MineResourceService(IDbContextFactory<PolarShadowDbContext> dbContextFactory) :base(dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task DeleteRootResourceAsync(long rootId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            using var trans = await dbContext.Database.BeginTransactionAsync();
            try
            {
                await dbContext.Resources.Where(f => f.Id == rootId).ExecuteDeleteAsync();
                await dbContext.Resources.Where(f => f.RootId == rootId).ExecuteDeleteAsync();
                await trans.CommitAsync();
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw;
            }
        }

        public async Task<ResourceModel> GetResourceAsync(long id)
        {
            using var dbcontent = await _dbContextFactory.CreateDbContextAsync();
            return await dbcontent.Resources.Where(f => f.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<ResourceModel>> GetRootChildrenAsync(long rootId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Resources.Where(f => f.RootId == rootId && f.Id != rootId).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<ResourceModel>> GetRootChildrenAsync(long rootId, int level)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Resources.Where(f=>f.RootId == rootId && f.Level == level).AsNoTracking().ToListAsync();
        }

        public async Task<int> GetRootChildrenCountAsync(long rootId, int level)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Resources.CountAsync(f=> f.RootId == rootId && f.Level == level);
        }

        public async Task<ResourceModel> GetRootResourceAsync(string resourceName, string site)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Resources.Where(f => f.ParentId == 0 && f.Name == resourceName && f.Site == site).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<ResourceModel>> GetRootResourcesAsync()
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Resources.Where(f => f.ParentId == 0).AsNoTracking().ToListAsync();
        }

        public async Task SaveResourceAsync(ResourceTreeNode tree)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            using var trans = await dbContext.Database.BeginTransactionAsync();

            try
            {
                SaveResourceAsync(tree, dbContext);

                await dbContext.SaveChangesAsync();

                await trans.CommitAsync();
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw;
            }
        }

        private void SaveResourceAsync(ResourceTreeNode tree, PolarShadowDbContext dbContext)
        {
            var now = DateTime.Now;

            foreach (var item in TreeEnumerable.EnumerateBreadthFirstWithParentChild(tree, t => t.Children))
            {
                if (item.Key == null && item.Value != null) // first
                {
                    if (item.Value.Id == 0)
                    {
                        item.Value.Id = IdGenerator.NextId();

                        dbContext.Resources.Add(item.Value);
                    }
                    else
                    {
                        dbContext.Resources.Update(item.Value);
                    }
                    item.Value.RootId = item.Value.Id;
                    item.Value.Level = 0;
                    item.Value.UpdateTime = now;
                }
                else if (item.Key != null && item.Value != null)
                {
                    if (item.Value.Id == 0)
                    {
                        item.Value.Id = IdGenerator.NextId();
                        dbContext.Resources.Add(item.Value);
                    }
                    else
                    {
                        dbContext.Resources.Update(item.Value);
                    }
                    item.Value.ParentId = item.Key.Id;
                    item.Value.RootId = item.Key.RootId;
                    item.Value.Level = item.Key.Level + 1;
                    item.Value.UpdateTime = now;
                }
            }
        }
    }
}
