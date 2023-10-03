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
    public class MineResourceService : IMineResourceService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbContextFactory;

        public MineResourceService(IDbContextFactory<PolarShadowDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task DeleteRootResourceAsync(int rootId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            using var trans = dbContext.Database.BeginTransaction();
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

        public async Task<ResourceModel> GetResourceAsync(int id)
        {
            using var dbcontent = _dbContextFactory.CreateDbContext();
            return await dbcontent.Resources.Where(f => f.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<ResourceModel>> GetRootChildrenAsync(int rootId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Resources.Where(f => f.RootId == rootId && f.Id != rootId).AsNoTracking().ToListAsync();
        }

        public async Task<ResourceModel> GetRootResourceAsync(string resourceName)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Resources.Where(f => f.ParentId == 0 && f.Name == resourceName).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<ResourceModel>> GetRootResourcesAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Resources.Where(f => f.ParentId == 0).AsNoTracking().ToListAsync();
        }

        public async Task SaveResourceAsync(ResourceTreeNode tree)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            using var trans = dbContext.Database.BeginTransaction();

            try
            {
                await dbContext.Resources.Where(f => f.Id == tree.Id).ExecuteDeleteAsync();
                await dbContext.Resources.Where(f => f.RootId == tree.Id).ExecuteDeleteAsync();

                dbContext.Resources.AddRange(TreeEnumerable.EnumerateDeepFirst(tree, t => t.Children));
                await dbContext.SaveChangesAsync();

                foreach (var item in TreeEnumerable.EnumerateBreadthFirstWithParentChild(tree, t => t.Children))
                {
                    if (item.Key == null && item.Value != null) // first
                    {
                        item.Value.RootId = item.Value.Id;
                        item.Value.Level = 0;
                    }

                    if (item.Key != null && item.Value != null)
                    {
                        item.Value.ParentId = item.Key.Id;
                        item.Value.RootId = item.Key.RootId;
                        item.Value.Level = item.Key.Level + 1;
                    }

                    if (item.Value != null)
                    {
                        dbContext.Resources.Update(item.Value);
                    }
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
